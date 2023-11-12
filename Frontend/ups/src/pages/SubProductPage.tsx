import { Dispatch, useEffect, useReducer, useState } from "react"
import { ExtendedSubProductDto, ParameterDto, ProductDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError, toastError } from "../helpers/ToastHelpers"
import { useParams } from "react-router-dom"
import { GetParameterTypeDisplayName, GetProductStatusDisplayName } from "../helpers/FormHelpers"
import { UnassignSubProductModal } from "../components/modals/UnassignSubProductModal"
import { Form } from "react-bootstrap"
import { AddOrEditParameterModal } from "../components/modals/AddOrEditParameterModal"
import { DeleteParameterModal } from "../components/modals/DeleteParameterModal"
import { AddOrEditSubProductModal } from "../components/modals/AddOrEditSubProductModal"
import { InfoRow } from "./ProductPage"

export default function SubProductPage() {
    const { id } = useParams();

    return  <>
        {!!id && <SubProductPageInner subProductId={+id} />}
    </>
}

type SubProductPageState = {
    subProduct: ExtendedSubProductDto | null,
    refreshSubProduct: boolean
    editSubProductModal: boolean
    addParameterModal: boolean,
    editParameterModal: ParameterDto | null
    deleteParameterModal: ParameterDto | null
    unassignSubProductModal: ProductDto | null,
}

const initalState: SubProductPageState = {
    subProduct: null,
    refreshSubProduct: true,
    editSubProductModal: false,
    addParameterModal: false,
    editParameterModal: null,
    deleteParameterModal: null,
    unassignSubProductModal: null
}

type SubProductPageAction =
    | { type: 'closeModal' }
    | { type: 'refreshSubProduct' }
    | { type: 'fetchedSubProduct', subProduct: ExtendedSubProductDto }
    | { type: 'editSubProductButton' }
    | { type: 'addParameterButton' }
    | { type: 'editParameterButton', parameter: ParameterDto }
    | { type: 'deleteOptionButton', optionId: number }
    | { type: 'deleteParameterButton', parameter: ParameterDto }
    | { type: 'unassignSubProductButton', product: ProductDto }

function reducer(state: SubProductPageState, action: SubProductPageAction): SubProductPageState {
    switch (action.type) {
        case 'closeModal':
            return { 
                ...state, editSubProductModal: false, unassignSubProductModal: null,
                addParameterModal: false, editParameterModal: null, deleteParameterModal: null
            }
        case 'refreshSubProduct':
            return { ...state, refreshSubProduct: true }
        case 'unassignSubProductButton':
            return { ...state, unassignSubProductModal: action.product }
        case 'addParameterButton':
            return { ...state, addParameterModal: true }
        case 'editParameterButton':
            return { ...state, editParameterModal: action.parameter }
        case 'editSubProductButton':
            return { ...state, editSubProductModal: true }
        case 'deleteParameterButton':
            return { ...state, deleteParameterModal: action.parameter }
        case 'deleteOptionButton':
            let subProduct = state.subProduct;
            if (!subProduct)
                return state;

            subProduct.parameters = subProduct?.parameters.map(p => 
                ({...p, options: p.options.filter(o => o.id !== action.optionId )}))
            return { ...state, subProduct: subProduct}
        case 'fetchedSubProduct':
            return { ...state, subProduct: action.subProduct, refreshSubProduct: false }
    }
}

type SubProductPageProps = {
    subProductId: number
}

const ParameterRow = ({ parameter, dispatch }: { parameter: ParameterDto, dispatch: Dispatch<SubProductPageAction> }) => {
    const [showOptions, setShowOptions] = useState(true);
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
                <label key="required" className="m-2 col-1 param-label">{parameter.required ? "Wymagany" : "Opcjonalny"}</label>
                <label key="hasOptions" className="m-2 col-2 param-label-options">
                    {parameter.options.length > 0 && !showOptions && "Dostępne opcje"}
                </label>
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
                {parameter.options.length > 1 && <button type="button" className="btn btn-sm btn-danger mt-1 mb-1 delete-option"
                    onClick={() => {
                        Api.DeleteOption({ optionId: o.id }).then(res => {
                            if (!res.success) {
                                toastError("Nie udało się usunąć opcji " + o.value);
                                dispatch({ type: "refreshSubProduct" })
                            }
                        })
                        dispatch({type: 'deleteOptionButton', optionId: o.id})
                    }}
                >
                    X
                </button>}
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
                                dispatch({ type: 'refreshSubProduct' })
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

export function SubProductPageInner({ subProductId }: SubProductPageProps) {
    const [state, dispatch] = useReducer(reducer, initalState);

    useEffect(() => {
        const fetchProduct = () => {
            Api.GetSubProduct({ subProductId: subProductId }).then(res => {
                if (res.success && res.data) {
                    dispatch({ type: 'fetchedSubProduct', subProduct: res.data.subProduct })
                }
                else toastDefaultError();
            })
        }

        if (state.refreshSubProduct)
            fetchProduct();
    }, [subProductId, state.refreshSubProduct])

    return <>
        {!!state.addParameterModal && <AddOrEditParameterModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            subProductId={subProductId}
        />}
        {!!state.editParameterModal && <AddOrEditParameterModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            subProductId={subProductId}
            editedParameter={state.editParameterModal}
        />}
        {!!state.deleteParameterModal && <DeleteParameterModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            deletedParameter={state.deleteParameterModal}
        />}
        {!!state.editSubProductModal && state.subProduct && <AddOrEditSubProductModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedSubProduct={state.subProduct}
        />}
        {!!state.unassignSubProductModal && state.subProduct && <UnassignSubProductModal
            onSuccess={() => {
                dispatch({ type: 'refreshSubProduct' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            product={state.unassignSubProductModal}
            subProduct={state.subProduct}
        />}
        <h3>{state.subProduct?.code} {state.subProduct?.name}</h3>
        <br />
        <Form.Label className="align-left">
            Dane
        </Form.Label>
        <div className="card">
            <form className="card-body product-info">
                <InfoRow value={state.subProduct?.code} name="Kod"/>
                <InfoRow value={state.subProduct?.name} name="Nazwa"/>
                <InfoRow value={state.subProduct?.basePrice?.toString()} name="Podstawowa cena"/>
            </form>
        </div>
        <Form.Label className="align-left">
            Opis
        </Form.Label>
        <textarea name="description" rows={4} disabled readOnly className="form-control" value={state.subProduct?.description ?? ""}/>
        <div className="align-left">
            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                dispatch({ type: 'editSubProductButton' })
            }}>
                Edytuj podprodukt
            </button>
        </div>
        <br/>
        <Form.Label className="align-left">
            Parametry
        </Form.Label>
        <div className="list-group">
            {state.subProduct?.parameters.map(p => {
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
                <h4>Przypisany do produktów</h4>
                <table className="table table-stripped">
                    <thead>
                        <tr>
                            <th>Kod</th>
                            <th>Nazwa</th>
                            <th>Status</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {state.subProduct?.products.map(p => {
                            return <tr key={p.id}>
                                <td>
                                    {p.code}
                                </td>
                                <td>
                                    {p.name}
                                </td>
                                <td>
                                    {GetProductStatusDisplayName(p.status)}
                                </td>
                                <td>
                                    <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => {
                                        dispatch({ type: 'unassignSubProductButton', product: p })
                                    }}>
                                        Usuń przypisanie
                                    </button>
                                </td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    </>
}
