import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { AuthHelpers } from "./AuthHelper";
import { Paths } from "../App";
import Header from "../components/Header";
import { Api } from "../api/Api";
import { UPSToastContainer, toastError } from "./ToastHelpers";
import { RoleEnum } from "../api/Dtos";

type AuththorizedPageProps = {
    page: JSX.Element,
    requiredRoles: RoleEnum[] | true
}

export function AuthorizedPage({ page, requiredRoles }: AuththorizedPageProps) {
    const nav = useNavigate();

    useEffect(() => {
        async function checkIfLoggedIn(){
            const sessionFront = AuthHelpers.IsLoggedIn();
            const sessionBack = await Api.Session();

            if (!(sessionBack && sessionFront)){
                Api.Logout()
                AuthHelpers.ClearAllData();
                nav(Paths.login);
            }

            if (requiredRoles !== true){
                if (!AuthHelpers.HasAnyRole([ ...requiredRoles, RoleEnum.Administrator ])){
                    nav(Paths.main)
                    toastError("Strona niedostępna dla posiadanych uprawnień")
                }
            }
        }

        checkIfLoggedIn();
    }, [nav, requiredRoles])

    return <>
        <header>
            <Header />
        </header>
        <main className='app container'>
            <UPSToastContainer/>
            {page}   
        </main>
    </>
}