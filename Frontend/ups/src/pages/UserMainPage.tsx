import { useEffect, useReducer } from "react"
import { AddOrEditUserModal } from "../components/modals/AddOrEditUserModal"
import { Api } from "../api/Api"
import { PaginationDto, RoleEnum, UserDto } from "../api/Dtos"
import { RoleEnumDisplayName } from "../helpers/FormHelpers"
import { DeleteUserModal } from "../components/modals/DeleteUserModal"

type UserMainPageState =  {
    addUserModalOpen: boolean,
    editUserModal: UserDto | null,
    deleteUserModal: UserDto | null,
    refresh: boolean
    users: UserDto[]
}

const initalState: UserMainPageState = {
    addUserModalOpen: false,
    refresh: true,
    users: [],
    editUserModal: null,
    deleteUserModal: null,
}

type UserMainPageAction =
    | { type: 'addUserButton' }
    | { type: 'editUserButton', user: UserDto }
    | { type: 'deleteUserButton', user: UserDto }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'fetchedUsers', users: UserDto[] }

function reducer (state: UserMainPageState, action: UserMainPageAction): UserMainPageState {
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
        case 'fetchedUsers':
            return { ...state, refresh: false, users: action.users }
    }
}

const defaultPagination: PaginationDto = {
    pageSize: 10,
    pageIndex: 0
}

export default function UserMainPage() {
    const [state, dispatch] = useReducer(reducer, initalState);

    const fetchData = () => {
        Api.ListUsers({ pagination: defaultPagination }).then(res => {
            if (res.success && res.data){
                dispatch({ type: 'fetchedUsers', users: res.data.users.items })
            }
            else {
                // generic toast
            }
        })
    }

    useEffect(() => {
        if (state.refresh)
            fetchData();
    }, [state.refresh])

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
                                return RoleEnumDisplayName(enumValue);
                            }).join(', ')}
                        </td>
                        <td className="col-2">
                            <button type="button" className="btn btn-sm btn-primary" onClick={() => {
                                dispatch({ type: 'editUserButton', user: u })
                            }}>
                                Edytuj
                            </button>
                            &nbsp;
                            &nbsp;
                            <button type="button" className="btn btn-sm btn-danger" onClick={() => {
                                dispatch({ type: 'deleteUserButton', user: u })
                            }}>
                                Usuń
                            </button>
                        </td>
                    </tr>
                })}
            </tbody>
        </table>
        <br/>
        <div className="col-sm-3">
            <button type="button" className="btn btn-primary" onClick={() => dispatch({ type: 'addUserButton' })}>
                Dodaj nowego użytkownika
            </button>
        </div>
    </>
}
