import { ToastContainer, toast } from "react-toastify";

export function UPSToastContainer() {
    return <div>
        <ToastContainer 
            position="top-right"
            autoClose={false}
            newestOnTop
            closeOnClick
            draggable={false}
            theme="dark"
        />
    </div>
}

export function toastInfo(msg: string) {
    toast(msg, {
        type: 'info'
    })
}

export function toastError(msg: string) {
    toast(msg, {
        type: 'error'
    })
}

export const toastDefaultError = () => toastError("Coś poszło nie tak")