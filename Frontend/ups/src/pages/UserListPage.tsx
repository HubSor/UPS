import { useCallback, useEffect, useReducer } from "react"
import { AddOrEditUserModal } from "../components/modals/AddOrEditUserModal"
import { Api } from "../api/Api"
import { ResultPaginationDto, RoleEnum, UserDto } from "../api/Dtos"
import { PaginationBar } from "../helpers/FormHelpers"
import { DeleteUserModal } from "../components/modals/DeleteUserModal"
import { toastDefaultError } from "../helpers/ToastHelpers"
import { GetRoleDisplayName } from "../helpers/EnumHelpers"

type UserListPageState =  {
    addUserModalOpen: boolean,
    editUserModal: UserDto | null,
    deleteUserModal: UserDto | null,
    refresh: boolean
    users: UserDto[]
    pagination: ResultPaginationDto
}

const initalState: UserListPageState = {
    addUserModalOpen: false,
    refresh: true,
    users: [],
    editUserModal: null,
    deleteUserModal: null,
    pagination: {
        pageSize: 10,
        pageIndex: 0,
        totalCount: 0,
        totalPages: 1,
        count: 0
    },
}

type UserListPageAction =
    | { type: 'addUserButton' }
    | { type: 'editUserButton', user: UserDto }
    | { type: 'deleteUserButton', user: UserDto }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'changedPage', pageIndex: number }
    | { type: 'fetchedUsers', users: UserDto[], pagination: ResultPaginationDto }

function reducer (state: UserListPageState, action: UserListPageAction): UserListPageState {
    switch(action.type){
        case 'addUserButton':
            return { ...state, addUserModalOpen: true }
        case 'editUserButton':
            return { ...state, editUserModal: action.user }
        case 'deleteUserButton':
            return { ...state, deleteUserModal: action.user }
        case 'closeModal':
            return { ...state, addUserModalOpen: false, editUserModal: null, deleteUserModal: null }
        case 'refresh':
            return { ...state, refresh: true }
        case 'changedPage':
            return { ...state, refresh: true, pagination: { ...state.pagination, pageIndex: action.pageIndex }}
        case 'fetchedUsers':
            return { ...state, refresh: false, users: action.users, pagination: action.pagination }
    }
}

export default function UserListPage() {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchData = useCallback(() => {
        Api.ListUsers({ pagination: state.pagination }).then(res => {
            if (res.success && res.data){
                dispatch({ type: 'fetchedUsers', users: res.data.users.items, pagination: res.data.users.pagination })
            }
            else toastDefaultError();
        })
    }, [state.pagination])

    useEffect(() => {
        if (state.refresh)
            fetchData();
    }, [state.refresh, fetchData])

    return <>
        {state.addUserModalOpen && <AddOrEditUserModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
        />}
        {!!state.editUserModal && <AddOrEditUserModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            editedUser={state.editUserModal}
        />}
        {!!state.deleteUserModal && <DeleteUserModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
            deletedUser={state.deleteUserModal}
        />}
        <h3>Użytkownicy</h3>
        <br/>
        <table className="table table-striped">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Nazwa</th>
                    <th scope="col">Role</th>
                    <th scope="col-2"></th>
                </tr>
            </thead>
            <tbody>
                {state.users.map(u => {
                    return <tr key={u.id}>
                        <td>
                            {u.username}
                        </td>
                        <td>
                            {u.roles.map(u => {
                                const enumKey = u as keyof typeof RoleEnum;
                                const enumValue = RoleEnum[enumKey]
                                return GetRoleDisplayName(enumValue);
                            }).join(', ')}
                        </td>
                        <td className="col-2">
                            <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => {
                                dispatch({ type: 'editUserButton', user: u })
                            }}>
                                Edytuj
                            </button>
                            &nbsp;
                            &nbsp;
                            <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => {
                                dispatch({ type: 'deleteUserButton', user: u })
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
            <button type="button" className="btn btn-primary" onClick={() => dispatch({ type: 'addUserButton' })}>
                Dodaj nowego użytkownika
            </button>
        </div>
    </>
}
