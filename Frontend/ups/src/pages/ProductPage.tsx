import { Dispatch, useCallback, useEffect, useReducer, useState } from "react"
import { ExtendedProductDto, ExtendedSubProductDto, ParameterDto, ResultPaginationDto, SubProductDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError, toastError } from "../helpers/ToastHelpers"
import { useParams } from "react-router-dom"
import { AddOrEditProductModal } from "../components/modals/AddOrEditProductModal"
import { GetParameterTypeDisplayName, GetProductStatusDisplayName, PaginationBar } from "../helpers/FormHelpers"
import { EditAssignedSubProductModal } from "../components/modals/EditAssignedSubProductModal"
import { AssignSubProductModal } from "../components/modals/AssignSubProductModal"
import { UnassignSubProductModal } from "../components/modals/UnassignSubProductModal"
import { Form } from "react-bootstrap"
import { AddOrEditParameterModal } from "../components/modals/AddOrEditParameterModal"
import { DeleteParameterModal } from "../components/modals/DeleteParameterModal"

export default function ProductPage() {
    const { id } = useParams();

    return  <>
        {!!id && <ProductPageInner productId={+id} />}
    </>
}

const defaultPagination: ResultPaginationDto = {
    pageSize: 10,
    pageIndex: 0,
    totalCount: 0,
    totalPages: 1,
    count: 0
}

type ProductPageState = {
    product: ExtendedProductDto | null,
    otherSubProducts: SubProductDto[]
    refreshSubProducts: boolean,
    refreshProduct: boolean
    otherSubProductsPagination: ResultPaginationDto
    showOtherSubProducts: boolean
    editProductModal: boolean
    editSubProductModal: ExtendedSubProductDto | null
    unassignSubProductModal: ExtendedSubProductDto | null,
    assignSubProductModal: SubProductDto | null,
    addParameterModal: boolean,
    editParameterModal: ParameterDto | null
    deleteParameterModal: ParameterDto | null
}

const initalState: ProductPageState = {
    product: null,
    refreshProduct: true,
    refreshSubProducts: false,
    otherSubProducts: [],
    otherSubProductsPagination: defaultPagination,
    showOtherSubProducts: false,
    editProductModal: false,
    editSubProductModal: null,
    unassignSubProductModal: null,
    assignSubProductModal: null,
    addParameterModal: false,
    editParameterModal: null,
    deleteParameterModal: null
}

type ProductPageAction =
    | { type: 'closeModal' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'refreshProduct' }
    | { type: 'refreshSubProducts' }
    | { type: 'fetchedSubProducts', subProducts: SubProductDto[], pagination: ResultPaginationDto }
    | { type: 'fetchedProduct', product: ExtendedProductDto }
    | { type: 'editProductButton' }
    | { type: 'addParameterButton' }
    | { type: 'editParameterButton', parameter: ParameterDto }
    | { type: 'showOtherSubProductsButton' }
    | { type: 'editSubProductButton', subProduct: ExtendedSubProductDto }
    | { type: 'unassignSubProductButton', subProduct: ExtendedSubProductDto }
    | { type: 'assignSubProductButton', subProduct: SubProductDto }
    | { type: 'deleteOptionButton', optionId: number }
    | { type: 'deleteParameterButton', parameter: ParameterDto }

function reducer(state: ProductPageState, action: ProductPageAction): ProductPageState {
    switch (action.type) {
        case 'showOtherSubProductsButton':
            return { ...state, showOtherSubProducts: true, refreshSubProducts: true }
        case 'closeModal':
            return { 
                ...state, editProductModal: false, editSubProductModal: null, 
                unassignSubProductModal: null, assignSubProductModal: null,
                addParameterModal: false, editParameterModal: null, deleteParameterModal: null
            }
        case 'editProductButton':
            return { ...state, editProductModal: true }
        case 'refreshProduct':
            return { ...state, refreshProduct: true }
        case 'refreshSubProducts':
            return { ...state, refreshSubProducts: true }
        case 'addParameterButton':
            return { ...state, addParameterModal: true }
        case 'editParameterButton':
            return { ...state, editParameterModal: action.parameter }
        case 'editSubProductButton':
            return { ...state, editSubProductModal: action.subProduct }
        case 'changedPage':
            return { ...state, refreshSubProducts: true, 
                otherSubProductsPagination: { ...state.otherSubProductsPagination, pageIndex: action.pageIndex } }
        case 'unassignSubProductButton':
            return { ...state, unassignSubProductModal: action.subProduct }
        case 'assignSubProductButton':
            return { ...state, assignSubProductModal: action.subProduct }
        case 'deleteParameterButton':
            return { ...state, deleteParameterModal: action.parameter }
        case 'deleteOptionButton':
            let product = state.product;
            if (!product)
                return state;

            product.parameters = product?.parameters.map(p => 
                ({...p, options: p.options.filter(o => o.id !== action.optionId )}))
            return { ...state, product: product}
        case 'fetchedSubProducts':
            return { 
                ...state,
                refreshSubProducts: false, 
                otherSubProducts: action.subProducts,
                otherSubProductsPagination: action.pagination 
            }
        case 'fetchedProduct':
            return { ...state, product: action.product, refreshProduct: false }
    }
}

type ProductPageProps = {
    productId: number
}

const InfoRow = ({name, value}: {name: string, value?: string}) => {
    return <div className="form-group row justify-content-center">
        <label className="col-sm-6 col-form-label info-label">{name}</label>
        <div className="col-6">
            <input type="text" readOnly className="form-control-plaintext info-value" value={value ?? ""} />
        </div>
    </div>
}

const ParameterRow = ({ parameter, dispatch }: { parameter: ParameterDto, dispatch: Dispatch<ProductPageAction> }) => {
    const [showOptions, setShowOptions] = useState(false);
    const [addingNew, setAddingNew] = useState(false);
    const [newOptionValue, setNewOptionValue] = useState("");

    return <div>
        <div className="list-group-item">
            <div className="d-flex justify-content-between" onClick={() => {
                setShowOptions(!showOptions)
            }}>
                <label key="id" className="m-2">{parameter.id}</label>
                <label key="name" className="m-2 col-4 param-label">{parameter.name}</label>
                <label key="type" className="m-2 col-2 param-label">{GetParameterTypeDisplayName(parameter.type)}</label>
                <label key="required" className="m-2 col-2 param-label">{parameter.required ? "Wymagany" : "Opcjonalny"}</label>
                <button className="m-1 col-1 btn btn-sm btn-outline-primary" type="button"
                    onClick={(e) => {
                        e.stopPropagation()
                        dispatch({ type: 'editParameterButton', parameter: parameter })
                    }}
                >
                    Edytuj
                </button>
                <button className="m-1 col-1 btn btn-sm btn-outline-danger" type="button"
                    onClick={(e) => {
                        e.stopPropagation();
                        dispatch({ type: 'deleteParameterButton', parameter: parameter })
                    }}
                >
                    Usuń
                </button>
            </div>
        </div>
        {parameter.options.length > 0 && showOptions && <div className="row row-cols-5 option-container m-2">
            {parameter.options.map((o, idx) => <div key={idx} className="col option-item d-flex justify-content-between align-items-center">
                <label className="m-1">{o.value}</label>
                <button type="button" className="btn btn-sm btn-danger mt-1 mb-1 delete-option"
                    onClick={() => {
                        Api.DeleteOption({ optionId: o.id }).then(res => {
                            if (!res.success) {
                                toastError("Nie udało się usunąć opcji " + o.value);
                                dispatch({ type: "refreshProduct" })
                            }
                        })
                        dispatch({type: 'deleteOptionButton', optionId: o.id})
                    }}
                >
                    X
                </button>
            </div>)}
            {!addingNew ? 
                <div key={-1} className="col">
                    <button className="m-1 btn btn-primary" type="button" onClick={() => {
                        setAddingNew(true)
                    }}>
                        Dodaj nową opcję
                    </button>
                </div> : 
                <form key={-2} className="col option-item d-flex justify-content-between align-items-center"
                    onSubmit={async (e) => {
                        e.preventDefault()
                        await Api.AddOption({ parameterId: parameter.id, value: newOptionValue }).then(res => {
                            if (res.success && res.data) {
                                dispatch({ type: 'refreshProduct' })
                                setAddingNew(false)
                                setNewOptionValue("")
                            }
                            else if (!!res.errors.optionId) {
                                toastError(res.errors.optionId[0])
                            }
                            else if (!!res.errors.value) {
                                toastError(res.errors.value[0])
                            }
                            else toastDefaultError();
                        })
                    }}
                >
                    <input type="text" name="value" className="form-control m-1"
                        onChange={(e) => e.target.value.length <= 256 ? setNewOptionValue(e.target.value) : undefined}
                        value={newOptionValue}
                        autoFocus
                    />
                    <button disabled={!newOptionValue}
                        type="submit" className="btn btn-sm btn-primary mt-1 mb-1 ml-1 delete-option">
                        Zapisz
                    </button>
                </form>
            }
        </div>}
    </div>
}

export function ProductPageInner({ productId }: ProductPageProps) {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchOtherSubProduts = useCallback(() => {
        Api.ListSubProducts({ pagination: state.otherSubProductsPagination, productId: productId }).then(res => {
            if (res.success && res.data) {
                dispatch({ 
                    type: 'fetchedSubProducts', 
                    subProducts: res.data.subProducts.items, 
                    pagination: res.data.subProducts.pagination 
                })
            }
            else toastDefaultError();
        })
    }, [productId, state.otherSubProductsPagination])

    useEffect(() => {
        const fetchProduct = () => {
            Api.GetProduct({ productId: productId }).then(res => {
                if (res.success && res.data) {
                    dispatch({ type: 'fetchedProduct', product: res.data.product })
                }
                else toastDefaultError();
            })
        }

        if (state.refreshProduct)
            fetchProduct();
    }, [productId, state.refreshProduct])

    useEffect(() => {
        if (state.refreshSubProducts && state.showOtherSubProducts)
            fetchOtherSubProduts();
    }, [state.showOtherSubProducts, state.refreshSubProducts, fetchOtherSubProduts])

    return <>
        {!!state.addParameterModal && <AddOrEditParameterModal
            onSuccess={() => {
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            productId={productId}
        />}
        {!!state.editParameterModal && <AddOrEditParameterModal
            onSuccess={() => {
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            productId={productId}
            editedParameter={state.editParameterModal}
        />}
        {!!state.deleteParameterModal && <DeleteParameterModal
            onSuccess={() => {
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            deletedParameter={state.deleteParameterModal}
        />}
        {!!state.assignSubProductModal && <AssignSubProductModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProducts' })
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            productId={productId}
            assignedSubProduct={state.assignSubProductModal}
        />}
        {!!state.unassignSubProductModal && state.product && <UnassignSubProductModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProducts' })
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            product={state.product}
            subProduct={state.unassignSubProductModal}
        />}
        {!!state.editProductModal && state.product && <AddOrEditProductModal
            onSuccess={() => {
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedProduct={state.product}
        />}
        {!!state.editSubProductModal && state.product && <EditAssignedSubProductModal
            onSuccess={() => {
                dispatch({ type: 'refreshProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedSubProduct={state.editSubProductModal}
            productId={productId}
        />}
        <h3>{state.product?.code} {state.product?.name}</h3>
        <br />
        <Form.Label className="align-left">
            Dane
        </Form.Label>
        <div className="card">
            <form className="card-body product-info">
                <InfoRow value={state.product?.code} name="Kod"/>
                <InfoRow value={state.product?.name} name="Nazwa"/>
                <InfoRow value={state.product?.basePrice?.toString()} name="Podstawowa cena"/>
                <InfoRow value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż"/>
                <InfoRow value={GetProductStatusDisplayName(state.product?.status)} name="Status"/>
            </form>
        </div>
        <Form.Label className="align-left">
            Opis
        </Form.Label>
        <textarea name="description" rows={4} disabled readOnly className="form-control" value={state.product?.description ?? ""}/>
        <div className="align-left">
            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                dispatch({ type: 'editProductButton' })
            }}>
                Edytuj produkt
            </button>
        </div>
        <br/>
        <Form.Label className="align-left">
            Parametry
        </Form.Label>
        <div className="list-group">
            {state.product?.parameters.map(p => {
                return <ParameterRow parameter={p} key={p.id}  dispatch={dispatch} />
            })}
        </div>
        <div className="align-left">
            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                dispatch({ type: 'addParameterButton' })
            }}>
                Dodaj parametr
            </button>
        </div>
        <br/>
        <br />
        <div className="row">
            <div className="col-lg-6 col-md-6 col-sm-12">
                <h4>Przypisane podprodukty</h4>
                <table className="table table-stripped">
                    <thead>
                        <tr>
                            <th>Kod</th>
                            <th>Nazwa</th>
                            <th>Cena</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {state.product?.subProducts.map(s => {
                            return <tr key={s.id}>
                                <td>
                                    {s.code}
                                </td>
                                <td>
                                    {s.name}
                                </td>
                                <td>
                                    {s.price}
                                </td>
                                <td>
                                    <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => {
                                        dispatch({ type: 'editSubProductButton', subProduct: s })
                                    }}>
                                        Edytuj
                                    </button>
                                    &nbsp;
                                    &nbsp;
                                    <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => {
                                        dispatch({ type: 'unassignSubProductButton', subProduct: s })
                                    }}>
                                        Usuń
                                    </button>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </div>
            <div className="col-lg-6 col-md-6 col-sm-12">
                {state.showOtherSubProducts ? <>
                    <h4>Dostępne podprodukty</h4>
                    <table className="table table-stripped">
                        <thead>
                            <tr>
                                <th>Kod</th>
                                <th>Nazwa</th>
                                <th>Bazowa cena</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {state.otherSubProducts?.map(s => {
                                return <tr key={s.id}>
                                    <td>
                                        {s.code}
                                    </td>
                                    <td>
                                        {s.name}
                                    </td>
                                    <td>
                                        {s.basePrice}
                                    </td>
                                    <td>
                                        <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => {
                                            dispatch({ type: 'assignSubProductButton', subProduct: s })
                                        }}>
                                            Przypisz
                                        </button>
                                    </td>
                                </tr>
                            })}
                        </tbody>
                    </table>
                    <PaginationBar currentIndex={state.otherSubProductsPagination.pageIndex} 
                        maxIndex={state.otherSubProductsPagination.totalPages - 1}
                        onNext={next => dispatch({ type: 'changedPage', pageIndex: next })}
                        onPrev={prev => dispatch({ type: 'changedPage', pageIndex: prev })}
                    />
                </> : <>
                    <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                        dispatch({ type: 'showOtherSubProductsButton' })
                    }}>
                        Pokaż dostępne podprodukty
                    </button>
                </>}
            </div>
        </div>
    </>
}
