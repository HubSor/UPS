import { useReducer } from "react"
import { AddUserModal } from "../components/modals/AddUserModal"

type UserMainPageState =  {
    addUserModalOpen: boolean,
    refresh: boolean
}

const initalState: UserMainPageState = {
    addUserModalOpen: false,
    refresh: false
}

type UserMainPageAction =
    | { type: 'addUserButton' }
    | { type: 'closeModal' }
    | { type: 'refresh' }
    | { type: 'fetched' }

function reducer (state: UserMainPageState, action: UserMainPageAction): UserMainPageState {
    switch(action.type){
        case 'addUserButton':
            return { ...state, addUserModalOpen: true }
        case 'closeModal':
            return { ...state, addUserModalOpen: false }
        case 'refresh':
            return { ...state, refresh: true }
        case 'fetched':
            return { ...state, refresh: false }
    }
}

export default function UserMainPage() {
    const [state, dispatch] = useReducer(reducer, initalState);


    return <>
        {state.addUserModalOpen && <AddUserModal 
            onSuccess={() => {
                dispatch({ type: 'refresh' })
                dispatch({ type: 'closeModal' })
            }}
            close={() => dispatch({ type: 'closeModal' })}
        />}
        <button type="button" onClick={() => dispatch({ type: 'addUserButton' })}>
            Dodaj użytkownika
        </button>
    </>
}
