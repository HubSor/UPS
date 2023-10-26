import { useCallback, useEffect, useReducer } from "react"
import { Api } from "../api/Api"
import { ProductDto, ProductStatusEnum, ResultPaginationDto } from "../api/Dtos"
import { PaginationBar } from "../helpers/FormHelpers"
import { DeleteUserModal } from "../components/modals/DeleteUserModal"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { AddOrEditProductModal } from "../components/modals/AddOrEditProductModal"

type ProductMainPageState =  {
    addProductModalOpen: boolean,
    editProductModal: ProductDto | null,
    deleteProductModal: ProductDto | null,
    refresh: boolean
    products: ProductDto[]
    pagination: ResultPaginationDto
    statuses: ProductStatusEnum[]
}

const initalState: ProductMainPageState = {
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

type ProductMainPageAction =
    | { type: 'addProductButton' }
    | { type: 'editProductButton', product: ProductDto }
    | { type: 'deleteProductButton', product: ProductDto }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'fetchedProducts', products: ProductDto[], pagination: ResultPaginationDto }

function reducer (state: ProductMainPageState, action: ProductMainPageAction): ProductMainPageState {
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

export default function ProductMainPage() {
    const [state, dispatch] = useReducer(reducer, initalState);

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
        {/* {!!state.deleteProductModal && <DeleteUserModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            deletedUser={state.deleteProductModal}
        />} */}
        <h3>Produkty</h3>
        <br/>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Kod</th>
                    <th scope="col">Nazwa</th>
                    <th scope="col">Status</th>
                    <th scope="col">Bazowa cena</th>
                    <th scope="col-2"></th>
                </tr>
            </thead>
            <tbody>
                {state.products.map(p => {
                    return <tr key={p.id}>
                        <td>
                            {p.code}
                        </td>
                        <td>
                            {p.name}
                        </td>
                        <td>
                            {p.status}
                        </td>
                        <td>
                            {p.basePrice}
                        </td>
                        <td className="col-2">
                            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                                dispatch({ type: 'editProductButton', product: p })
                            }}>
                                Edytuj
                            </button>
                            &nbsp;
                            &nbsp;
                            <button type="button" className="btn btn-sm btn-danger" onClick={() => {
                                dispatch({ type: 'deleteProductButton', product: p })
                            }}>
                                Usuń
                            </button>
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
        <div className="col-sm-3">
            <button type="button" className="btn btn-primary" onClick={() => dispatch({ type: 'addProductButton' })}>
                Dodaj nowy produkt
            </button>
        </div>
    </>
}
