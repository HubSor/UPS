import { useEffect, useReducer } from "react"
import { ExtendedSubProductDto, ParameterDto, ProductDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { useParams } from "react-router-dom"
import { GetProductStatusDisplayName } from "../helpers/FormHelpers"
import { UnassignSubProductModal } from "../components/modals/UnassignSubProductModal"
import { Form } from "react-bootstrap"
import { AddOrEditParameterModal } from "../components/modals/AddOrEditParameterModal"
import { DeleteParameterModal } from "../components/modals/DeleteParameterModal"
import { AddOrEditSubProductModal } from "../components/modals/AddOrEditSubProductModal"
import { InfoRow } from "../components/InfoRow"
import { ParameterRow } from "../components/ParameterRow"

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
                return <ParameterRow parameter={p} key={p.id}
                    refresh={() => dispatch({ type: 'refreshSubProduct' })}
                    deleteParameter={(p) => dispatch({ type: 'deleteParameterButton', parameter: p })}
                    editParameter={(p) => dispatch({ type: 'editParameterButton', parameter: p })}
                    deleteOption={(id) => dispatch({ type: 'deleteOptionButton', optionId: id })}
                />
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
