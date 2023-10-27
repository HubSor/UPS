import { useCallback, useEffect, useReducer } from "react"
import { Api } from "../api/Api"
import { SubProductDto, ResultPaginationDto } from "../api/Dtos"
import { PaginationBar } from "../helpers/FormHelpers"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { AddOrEditSubProductModal } from "../components/modals/AddOrEditSubProductModal"
import { DeleteSubProductModal } from "../components/modals/DeleteSubProductModal"

type SubProductMainPageState =  {
    addSubProductModalOpen: boolean,
    editSubProductModal: SubProductDto | null,
    deleteSubProductModal: SubProductDto | null,
    refresh: boolean
    subProducts: SubProductDto[]
    pagination: ResultPaginationDto
}

const initalState: SubProductMainPageState = {
    addSubProductModalOpen: false,
    refresh: true,
    subProducts: [],
    editSubProductModal: null,
    deleteSubProductModal: null,
    pagination: {
        pageSize: 10,
        pageIndex: 0,
        totalCount: 0,
        totalPages: 1,
        count: 0
    },
}

type SubSubProductMainPageAction =
    | { type: 'addSubProductButton' }
    | { type: 'editSubProductButton', subProduct: SubProductDto }
    | { type: 'deleteSubProductButton', subProduct: SubProductDto }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'fetchedSubProducts', subProducts: SubProductDto[], pagination: ResultPaginationDto }

function reducer (state: SubProductMainPageState, action: SubSubProductMainPageAction): SubProductMainPageState {
    switch(action.type){
        case 'addSubProductButton':
            return { ...state, addSubProductModalOpen: true }
        case 'editSubProductButton':
            return { ...state, editSubProductModal: action.subProduct }
        case 'deleteSubProductButton':
            return { ...state, deleteSubProductModal: action.subProduct }
        case 'closeModal':
            return { ...state, addSubProductModalOpen: false, editSubProductModal: null, deleteSubProductModal: null }
        case 'refresh':
            return { ...state, refresh: true }
        case 'changedPage':
            return { ...state, refresh: true, pagination: { ...state.pagination, pageIndex: action.pageIndex }}
        case 'fetchedSubProducts':
            return { ...state, refresh: false, subProducts: action.subProducts, pagination: action.pagination }
    }
}

export default function SubSubProductMainPage() {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchData = useCallback(() => {
        Api.ListSubProducts({ pagination: state.pagination }).then(res => {
            if (res.success && res.data){
                dispatch({ 
                    type: 'fetchedSubProducts', subProducts: res.data.subProducts.items, 
                    pagination: res.data.subProducts.pagination 
                })
            }
            else toastDefaultError();
        })
    }, [state.pagination])

    useEffect(() => {
        if (state.refresh)
            fetchData();
    }, [state.refresh, fetchData])

    return <>
        {state.addSubProductModalOpen && <AddOrEditSubProductModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
        />}
        {!!state.editSubProductModal && <AddOrEditSubProductModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedSubProduct={state.editSubProductModal}
        />}
        {!!state.deleteSubProductModal && <DeleteSubProductModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            deletedSubProduct={state.deleteSubProductModal}
        />}
        <h3>Podprodukty</h3>
        <br/>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Kod</th>
                    <th scope="col">Nazwa</th>
                    <th scope="col">Bazowa cena</th>
                    <th scope="col-2"></th>
                </tr>
            </thead>
            <tbody>
                {state.subProducts.map(p => {
                    return <tr key={p.id}>
                        <td>
                            {p.code}
                        </td>
                        <td>
                            {p.name}
                        </td>
                        <td>
                            {p.basePrice}
                        </td>
                        <td className="col-2">
                            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                                dispatch({ type: 'editSubProductButton', subProduct: p })
                            }}>
                                Edytuj
                            </button>
                            &nbsp;
                            &nbsp;
                            <button type="button" className="btn btn-sm btn-danger" onClick={() => {
                                dispatch({ type: 'deleteSubProductButton', subProduct: p })
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
            <button type="button" className="btn btn-primary" onClick={() => dispatch({ type: 'addSubProductButton' })}>
                Dodaj nowy podprodukt
            </button>
        </div>
    </>
}
