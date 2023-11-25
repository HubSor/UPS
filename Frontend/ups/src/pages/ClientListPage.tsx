import { useCallback, useEffect, useReducer } from "react"
import { Api } from "../api/Api"
import { CompanyClientDto, PersonClientDto, ResultPaginationDto } from "../api/Dtos"
import { PaginationBar, defaultPagination } from "../helpers/FormHelpers"
import { toastDefaultError } from "../helpers/ToastHelpers"

type ClientListPageState =  {
    refreshPeople: boolean
    refreshCompanies: boolean
    people: PersonClientDto[]
    companies: CompanyClientDto[]
    peoplePagination: ResultPaginationDto
    companiesPagination: ResultPaginationDto
}

const initalState: ClientListPageState = {
    refreshPeople: true,
    refreshCompanies: true,
    people: [],
    companies: [],
    peoplePagination: defaultPagination,
    companiesPagination: defaultPagination,
}

type ClientListPageAction =
    | { type: 'changedPeoplePage', pageIndex: number }
    | { type: 'changedCompaniesPage', pageIndex: number }
    | { type: 'fetchedPeople', people: PersonClientDto[], pagination: ResultPaginationDto }
    | { type: 'fetchedCompanies', companies: CompanyClientDto[], pagination: ResultPaginationDto }

function reducer (state: ClientListPageState, action: ClientListPageAction): ClientListPageState {
    switch(action.type){
        case 'changedPeoplePage':
            return { ...state, refreshPeople: true, peoplePagination: { ...state.peoplePagination, pageIndex: action.pageIndex }}
        case 'changedCompaniesPage':
            return { ...state, refreshCompanies: true, companiesPagination: { ...state.companiesPagination, pageIndex: action.pageIndex } }
        case 'fetchedPeople':
            return { ...state, refreshPeople: false, people: action.people, peoplePagination: action.pagination }
        case 'fetchedCompanies':
            return { ...state, refreshCompanies: false, companies: action.companies, companiesPagination: action.pagination }
    }
}

export default function ClientListPage() {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchPeopleData = useCallback(() => {
        Api.ListPersonClients({ pagination: state.peoplePagination }).then(res => {
            if (res.success && res.data){
                dispatch({ 
                    type: 'fetchedPeople', people: res.data.clients.items, 
                    pagination: res.data.clients.pagination 
                })
            }
            else toastDefaultError();
        })
    }, [state.peoplePagination])

    const fetchCompaniesData = useCallback(() => {
        Api.ListCompanyClients({ pagination: state.companiesPagination }).then(res => {
            if (res.success && res.data) {
                dispatch({
                    type: 'fetchedCompanies', companies: res.data.clients.items,
                    pagination: res.data.clients.pagination
                })
            }
            else toastDefaultError();
        })
    }, [state.companiesPagination])

    useEffect(() => {
        if (state.refreshPeople)
            fetchPeopleData();
    }, [state.refreshPeople, fetchPeopleData])

    useEffect(() => {
        if (state.refreshCompanies)
            fetchCompaniesData();
    }, [state.refreshCompanies, fetchCompaniesData])

    return <>
        <h3>Klienci</h3>
        <br/>
        <h4>Osoby</h4>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Id</th>
                    <th scope="col">Imię</th>
                    <th scope="col">Nazwisko</th>
                    <th scope="col">PESEL</th>
                    <th scope="col">Telefon</th>
                    <th scope="col">E-mail</th>
                </tr>
            </thead>
            <tbody>
                {state.people.map(p => <tr key={p.id}>
                    <td>
                        {p.id}
                    </td>
                    <td>
                        {p.firstName}
                    </td>
                    <td>
                        {p.lastName}
                    </td>
                    <td>
                        {p.pesel}
                    </td>
                    <td>
                        {p.phoneNumber}
                    </td>
                    <td>
                        {p.email}
                    </td>
                </tr>)}
            </tbody>
        </table>
        <PaginationBar currentIndex={state.peoplePagination.pageIndex} maxIndex={state.peoplePagination.totalPages - 1} 
            onNext={next => dispatch({ type: 'changedPeoplePage', pageIndex: next })}
            onPrev={prev => dispatch({ type: 'changedPeoplePage', pageIndex: prev })}
        />
        <br/>
        <hr/>
        <h4>Firmy</h4>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Id</th>
                    <th scope="col">Nazwa</th>
                    <th scope="col">REGON</th>
                    <th scope="col">NIP</th>
                    <th scope="col">Telefon</th>
                    <th scope="col">E-mail</th>
                </tr>
            </thead>
            <tbody>
                {state.companies.map(p => <tr key={p.id}>
                    <td>
                        {p.id}
                    </td>
                    <td>
                        {p.companyName}
                    </td>
                    <td>
                        {p.regon}
                    </td>
                    <td>
                        {p.nip}
                    </td>
                    <td>
                        {p.phoneNumber}
                    </td>
                    <td>
                        {p.email}
                    </td>
                </tr>)}
            </tbody>
        </table>
        <PaginationBar currentIndex={state.companiesPagination.pageIndex} maxIndex={state.companiesPagination.totalPages - 1}
            onNext={next => dispatch({ type: 'changedCompaniesPage', pageIndex: next })}
            onPrev={prev => dispatch({ type: 'changedCompaniesPage', pageIndex: prev })}
        />
        <br />
    </>
}
