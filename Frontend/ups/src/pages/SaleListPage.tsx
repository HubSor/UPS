import { useCallback, useEffect, useReducer } from "react"
import { Api } from "../api/Api"
import { ResultPaginationDto, SaleDto } from "../api/Dtos"
import { PaginationBar, defaultPagination } from "../helpers/FormHelpers"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { useNavigate } from "react-router-dom"
import { Paths } from "../App"

type SaleListPageState =  {
    refresh: boolean
    sales: SaleDto[]
    pagination: ResultPaginationDto
}

const initalState: SaleListPageState = {
    refresh: true,
    sales: [],
    pagination: defaultPagination
}

type SaleListPageAction =
    | { type: 'refresh' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'fetchedSales', sales: SaleDto[], pagination: ResultPaginationDto }

function reducer (state: SaleListPageState, action: SaleListPageAction): SaleListPageState {
    switch(action.type){
        case 'refresh':
            return { ...state, refresh: true }
        case 'changedPage':
            return { ...state, refresh: true, pagination: { ...state.pagination, pageIndex: action.pageIndex }}
        case 'fetchedSales':
            return { ...state, refresh: false, sales: action.sales, pagination: action.pagination }
    }
}

export default function SaleListPage() {
    const [state, dispatch] = useReducer(reducer, initalState);
    const nav = useNavigate();

    const fetchData = useCallback(() => {
        Api.ListSales({ pagination: state.pagination }).then(res => {
            if (res.success && res.data){
                dispatch({ 
                    type: 'fetchedSales', sales: res.data.sales.items, 
                    pagination: res.data.sales.pagination 
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
        <h3>Transakcje</h3>
        <br/>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Id</th>
                    <th scope="col">Produkt</th>
                    <th scope="col-3">Podprodukty</th>
                    <th scope="col">Należność</th>
                    <th scope="col">Klient</th>
                    <th scope="col">Data</th>
                </tr>
            </thead>
            <tbody>
                {state.sales.map(s => <tr key={s.saleId} onClick={() => {
                        nav(Paths.sale.replace(":id", s.saleId.toString()))
                    }}>
                        <td>
                            {s.saleId}
                        </td>
                        <td>
                            {s.productCode}
                        </td>
                        <td className="col-3">
                            {s.subProductCodes.join(',')}
                        </td>
                        <td>
                            {s.totalPrice.toFixed(2).replace('.', ',')}
                        </td>
                        <td>
                            {s.clientName}
                        </td>
                        <td>
                            {s.saleTime}
                        </td>
                    </tr>
                )}
            </tbody>
        </table>
        <PaginationBar currentIndex={state.pagination.pageIndex} maxIndex={state.pagination.totalPages - 1} 
            onNext={next => dispatch({ type: 'changedPage', pageIndex: next })}
            onPrev={prev => dispatch({ type: 'changedPage', pageIndex: prev })}
        />
        <br/>
    </>
}
