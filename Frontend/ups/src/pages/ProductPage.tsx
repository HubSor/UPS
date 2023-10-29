import { useCallback, useEffect, useReducer } from "react"
import { ExtendedProductDto, ExtendedSubProductDto, ResultPaginationDto, SubProductDto } from "../api/Dtos"
import { Api } from "../api/Api"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { useParams } from "react-router-dom"
import { AddOrEditProductModal } from "../components/modals/AddOrEditProductModal"
import { GetProductStatusDisplayName } from "../helpers/FormHelpers"
import { EditAssignedSubProductModal } from "../components/modals/EditAssignedSubProductModal"

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
    refresh: boolean
    subProductsPagination: ResultPaginationDto
    otherSubProductsPagination: ResultPaginationDto
    showOtherSubProducts: boolean
    editProductModal: boolean
    editSubProductModal: ExtendedSubProductDto | null
    unassignSubProductModal: ExtendedSubProductDto | null,
}

const initalState: ProductPageState = {
    product: null,
    refresh: true,
    otherSubProducts: [],
    subProductsPagination: defaultPagination,
    otherSubProductsPagination: defaultPagination,
    showOtherSubProducts: false,
    editProductModal: false,
    editSubProductModal: null,
    unassignSubProductModal: null,
}

type ProductPageAction =
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'fetchedSubProducts', subProducts: SubProductDto[], pagination: ResultPaginationDto }
    | { type: 'fetchedProduct', product: ExtendedProductDto }
    | { type: 'editProductButton' }
    | { type: 'editSubProductButton', subProduct: ExtendedSubProductDto }
    | { type: 'unassignSubProductButton', subProduct: ExtendedSubProductDto }

function reducer(state: ProductPageState, action: ProductPageAction): ProductPageState {
    switch (action.type) {
        case 'closeModal':
            return { ...state, editProductModal: false, editSubProductModal: null, unassignSubProductModal: null }
        case 'editProductButton':
            return { ...state, editProductModal: true }
        case 'refresh':
            return { ...state, refresh: true }
        case 'editSubProductButton':
            return { ...state, editSubProductModal: action.subProduct }
        case 'unassignSubProductButton':
            return { ...state, unassignSubProductModal: action.subProduct }
        case 'fetchedSubProducts':
            return { 
                ...state,
                refresh: false, 
                otherSubProducts: action.subProducts,
                otherSubProductsPagination: action.pagination 
            }
        case 'fetchedProduct':
            return { ...state, product: action.product, refresh: false }
    }
}

type ProductPageProps = {
    productId: number
}

export function ProductPageInner({ productId }: ProductPageProps) {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchOtherSubProduts = useCallback(() => {
        Api.ListSubProducts({ pagination: state.otherSubProductsPagination, productId: productId }).then(res => {
            if (res.success && res.data) {
                dispatch({ type: 'fetchedSubProducts', subProducts: res.data.subProducts.items, pagination: res.data.subProducts.pagination })
            }
            else toastDefaultError();
        })
    }, [productId, state.otherSubProductsPagination])

    const fetchProduct = useCallback(() => {
        Api.GetProduct({ productId: productId }).then(res => {
            if (res.success && res.data) {
                dispatch({ type: 'fetchedProduct', product: res.data.product })
            }
            else toastDefaultError();
        })
    }, [productId])

    useEffect(() => {
        if (state.refresh)
            fetchProduct();
    }, [productId, state.refresh, fetchProduct])

    useEffect(() => {
        if (state.refresh && state.showOtherSubProducts)
            fetchOtherSubProduts();
    }, [state.showOtherSubProducts, state.refresh, fetchOtherSubProduts])

    return <>
        {!!state.editProductModal && state.product && <AddOrEditProductModal
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedProduct={state.product}
        />}
        {!!state.editSubProductModal && state.product && <EditAssignedSubProductModal
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedSubProduct={state.editSubProductModal}
            productId={productId}
        />}
        <h3>{state.product?.code} {state.product?.name}</h3>
        <br />
        <div className="product-info">
            <form>
                <div className="form-group row">
                    <label className="col-sm-2 col-form-label">Nazwa</label>
                    <div className="col-sm-10">
                        <input type="text" readOnly className="form-control-plaintext" value={state.product?.name}/>
                    </div>
                </div>
                <div className="form-group row">
                    <label className="col-sm-2 col-form-label">Kod</label>
                    <div className="col-sm-10">
                        <input type="text" readOnly className="form-control-plaintext" value={state.product?.code} />
                    </div>
                </div>
                <div className="form-group row">
                    <label className="col-sm-2 col-form-label">Bazowa cena</label>
                    <div className="col-sm-10">
                        <input type="text" readOnly className="form-control-plaintext" value={state.product?.basePrice} />
                    </div>
                </div>
                <div className="form-group row">
                    <label className="col-sm-2 col-form-label">Anonimowa sprzedaż</label>
                    <div className="col-sm-10">
                        <input type="text" readOnly className="form-control-plaintext" 
                            value={state.product?.anonymousSaleAllowed ? "TAK" : "NIE"} 
                        />
                    </div>
                </div>
                <div className="form-group row">
                    <label className="col-sm-2 col-form-label">Status</label>
                    <div className="col-sm-10">
                        <input type="text" readOnly className="form-control-plaintext" 
                            value={GetProductStatusDisplayName(state.product?.status)} 
                        />
                    </div>
                </div>
                <div className="form-group row">
                    <label className="col-sm-2 col-form-label">Opis</label>
                    <div className="col-sm-10">
                        <input type="text" readOnly className="form-control-plaintext" value={state.product?.description} />
                    </div>
                </div>
            </form>
            <div className="align-left">
                <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                    dispatch({ type: 'editProductButton' })
                }}>
                    Edytuj produkt
                </button>
            </div>
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
            {productId === 59 && <div className="col-lg-6 col-md-6 col-sm-12">
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
                        
                    </tbody>
                </table>
            </div>}
        </div>
    </>
}
