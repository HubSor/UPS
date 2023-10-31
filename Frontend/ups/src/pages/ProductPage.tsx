import { useCallback, useEffect, useReducer } from "react"
import { ExtendedProductDto, ExtendedSubProductDto, ResultPaginationDto, SubProductDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { useParams } from "react-router-dom"
import { AddOrEditProductModal } from "../components/modals/AddOrEditProductModal"
import { GetProductStatusDisplayName, PaginationBar } from "../helpers/FormHelpers"
import { EditAssignedSubProductModal } from "../components/modals/EditAssignedSubProductModal"
import { AssignSubProductModal } from "../components/modals/AssignSubProductModal"
import { UnassignSubProductModal } from "../components/modals/UnassignSubProductModal"
import { Form } from "react-bootstrap"

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
}

type ProductPageAction =
    | { type: 'closeModal' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'refreshProduct' }
    | { type: 'refreshSubProducts' }
    | { type: 'fetchedSubProducts', subProducts: SubProductDto[], pagination: ResultPaginationDto }
    | { type: 'fetchedProduct', product: ExtendedProductDto }
    | { type: 'editProductButton' }
    | { type: 'showOtherSubProductsButton' }
    | { type: 'editSubProductButton', subProduct: ExtendedSubProductDto }
    | { type: 'unassignSubProductButton', subProduct: ExtendedSubProductDto }
    | { type: 'assignSubProductButton', subProduct: SubProductDto }

function reducer(state: ProductPageState, action: ProductPageAction): ProductPageState {
    switch (action.type) {
        case 'showOtherSubProductsButton':
            return { ...state, showOtherSubProducts: true, refreshSubProducts: true }
        case 'closeModal':
            return { 
                ...state, editProductModal: false, editSubProductModal: null, 
                unassignSubProductModal: null, assignSubProductModal: null 
            }
        case 'editProductButton':
            return { ...state, editProductModal: true }
        case 'refreshProduct':
            return { ...state, refreshProduct: true }
        case 'refreshSubProducts':
            return { ...state, refreshSubProducts: true }
        case 'editSubProductButton':
            return { ...state, editSubProductModal: action.subProduct }
        case 'changedPage':
            return { ...state, refreshSubProducts: true, 
                otherSubProductsPagination: { ...state.otherSubProductsPagination, pageIndex: action.pageIndex } }
        case 'unassignSubProductButton':
            return { ...state, unassignSubProductModal: action.subProduct }
        case 'assignSubProductButton':
            return { ...state, assignSubProductModal: action.subProduct }
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
        <div className="card">
            <form className="card-body product-info">
                <InfoRow value={state.product?.code} name="Kod"/>
                <InfoRow value={state.product?.name} name="Nazwa"/>
                <InfoRow value={state.product?.basePrice?.toString()} name="Podstawowa cena"/>
                <InfoRow value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} name="Anonimowa sprzedaż"/>
                <InfoRow value={GetProductStatusDisplayName(state.product?.status)} name="Status"/>
            </form>
        </div>
        <Form.Group className="mb-3">
            <Form.Label className="align-left">
                Opis
            </Form.Label>
            <textarea name="description" rows={4} disabled readOnly className="form-control" value={state.product?.description ?? ""}/>
        </Form.Group>
        <div className="align-left">
            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                dispatch({ type: 'editProductButton' })
            }}>
                Edytuj produkt
            </button>
        </div>
        <br/>
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
                                    <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                                        dispatch({ type: 'editSubProductButton', subProduct: s })
                                    }}>
                                        Edytuj
                                    </button>
                                    &nbsp;
                                    &nbsp;
                                    <button type="button" className="btn btn-sm btn-danger" onClick={() => {
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
                                        <button type="button" className="btn btn-sm btn-primary" onClick={() => {
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
