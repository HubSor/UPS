import { useCallback, useEffect, useReducer } from "react"
import { Api } from "../api/Api"
import { ProductDto, ProductStatusEnum, ResultPaginationDto } from "../api/Dtos"
import { PaginationBar, taxText } from "../helpers/FormHelpers"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { AddOrEditProductModal } from "../components/modals/AddOrEditProductModal"
import { DeleteProductModal } from "../components/modals/DeleteProductModal"
import { useNavigate } from "react-router-dom"
import { Paths } from "../App"
import { AuthHelpers } from "../helpers/AuthHelper"
import { GetProductStatusDisplayName } from "../helpers/EnumHelpers"

type ProductListPageState =  {
    addProductModalOpen: boolean,
    editProductModal: ProductDto | null,
    deleteProductModal: ProductDto | null,
    refresh: boolean
    products: ProductDto[]
    pagination: ResultPaginationDto
    statuses: ProductStatusEnum[]
}

const initalState: ProductListPageState = {
    addProductModalOpen: false,
    refresh: true,
    products: [],
    statuses: [ ProductStatusEnum.NotOffered, ProductStatusEnum.Offered, ProductStatusEnum.Withdrawn ],
    editProductModal: null,
    deleteProductModal: null,
    pagination: {
        pageSize: 10,
        pageIndex: 0,
        totalCount: 0,
        totalPages: 1,
        count: 0
    },
}

type ProductListPageAction =
    | { type: 'addProductButton' }
    | { type: 'editProductButton', product: ProductDto }
    | { type: 'deleteProductButton', product: ProductDto }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'fetchedProducts', products: ProductDto[], pagination: ResultPaginationDto }

function reducer (state: ProductListPageState, action: ProductListPageAction): ProductListPageState {
    switch(action.type){
        case 'addProductButton':
            return { ...state, addProductModalOpen: true }
        case 'editProductButton':
            return { ...state, editProductModal: action.product }
        case 'deleteProductButton':
            return { ...state, deleteProductModal: action.product }
        case 'closeModal':
            return { ...state, addProductModalOpen: false, editProductModal: null, deleteProductModal: null }
        case 'refresh':
            return { ...state, refresh: true }
        case 'changedPage':
            return { ...state, refresh: true, pagination: { ...state.pagination, pageIndex: action.pageIndex }}
        case 'fetchedProducts':
            return { ...state, refresh: false, products: action.products, pagination: action.pagination }
    }
}

export default function ProductListPage() {
    const [state, dispatch] = useReducer(reducer, initalState);
    const nav = useNavigate();

    const fetchData = useCallback(() => {
        Api.ListProducts({ pagination: state.pagination, statuses: state.statuses }).then(res => {
            if (res.success && res.data){
                dispatch({ type: 'fetchedProducts', products: res.data.products.items, pagination: res.data.products.pagination })
            }
            else toastDefaultError();
        })
    }, [state.pagination, state.statuses])

    useEffect(() => {
        if (state.refresh)
            fetchData();
    }, [state.refresh, fetchData])

    const hasProductRoles = AuthHelpers.HasProductRoles(); 

    return <>
        {state.addProductModalOpen && <AddOrEditProductModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
        />}
        {!!state.editProductModal && <AddOrEditProductModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedProduct={state.editProductModal}
        />}
        {!!state.deleteProductModal && <DeleteProductModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            deletedProduct={state.deleteProductModal}
        />}
        <h3>Produkty</h3>
        <br/>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Kod</th>
                    <th scope="col">Nazwa</th>
                    <th scope="col">Status</th>
                    <th scope="col">Podstawowa cena</th>
                    <th scope="col">Stawka podatku</th>
                    <th scope="col-3"></th>
                </tr>
            </thead>
            <tbody>
                {state.products.map(p => {
                    return <tr key={p.id} onClick={() => {
                        if (hasProductRoles)
                            nav(Paths.product.replace(":id", p.id.toString()))
                    }} data-toggle="tooltip" data-placement="top" title={p.description}>
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
                            {p.basePrice}
                        </td>
                        <td>
                            {taxText(p.taxRate)}
                        </td>
                        <td className="col-2">
                            {hasProductRoles && <>
                                <button type="button" className="btn btn-sm btn-outline-primary" onClick={(e) => {
                                    e.stopPropagation()
                                    dispatch({ type: 'editProductButton', product: p })
                                }}>
                                    Edytuj
                                </button>
                                &nbsp;
                                <button type="button" className="btn btn-sm btn-outline-danger" onClick={(e) => {
                                    e.stopPropagation()
                                    dispatch({ type: 'deleteProductButton', product: p })
                                }}>
                                    Usuń
                                </button>
                            </>}
                        </td>
                    </tr>
                })}
            </tbody>
        </table>
        <PaginationBar currentIndex={state.pagination.pageIndex} maxIndex={state.pagination.totalPages - 1} 
            onNext={next => dispatch({ type: 'changedPage', pageIndex: next })}
            onPrev={prev => dispatch({ type: 'changedPage', pageIndex: prev })}
        />
        <br/>
        {hasProductRoles && <div className="col-sm-3">
            <button type="button" className="btn btn-primary" onClick={() => dispatch({ type: 'addProductButton' })}>
                Dodaj nowy produkt
            </button>
        </div>}
    </>
}
