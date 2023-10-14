import { useEffect, useReducer } from "react"
import { AddUserModal } from "../components/modals/AddUserModal"
import { Api } from "../api/Api"
import { PaginationDto, RoleEnum, UserDto } from "../api/Dtos"
import { RoleEnumDisplayName } from "../helpers/FormHelpers"

type UserMainPageState =  {
    addUserModalOpen: boolean,
    editUserModal: number | null,
    deleteUserModal: number | null,
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
    | { type: 'editUserButton', id: number }
    | { type: 'deleteUserButton', id: number }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'fetchedUsers', users: UserDto[] }

function reducer (state: UserMainPageState, action: UserMainPageAction): UserMainPageState {
    switch(action.type){
        case 'addUserButton':
            return { ...state, addUserModalOpen: true }
        case 'editUserButton':
            return { ...state, editUserModal: action.id }
        case 'deleteUserButton':
            return { ...state, deleteUserModal: action.id }
        case 'closeModal':
            return { ...state, addUserModalOpen: false, editUserModal: null }
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
        {state.addUserModalOpen && <AddUserModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
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
                                dispatch({ type: 'editUserButton', id: u.id })
                            }}>
                                Edytuj
                            </button>
                            &nbsp;
                            &nbsp;
                            <button type="button" className="btn btn-sm btn-danger" onClick={() => {
                                dispatch({ type: 'deleteUserButton', id: u.id })
                            }}>
                                Usuń
                            </button>
                        </td>
                    </tr>
                })}
            </tbody>
        </table>
    </>
}
