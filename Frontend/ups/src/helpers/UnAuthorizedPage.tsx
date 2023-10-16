import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { AuthHelpers } from "./AuthHelper";
import { Paths } from "../App";
import { Api } from "../api/Api";
import { UPSToastContainer } from "./ToastHelpers";

type UnAuththorizedPageProps = {
    page: JSX.Element
}

export function UnAuthorizedPage({ page }: UnAuththorizedPageProps) {
    const nav = useNavigate();

    useEffect(() => {
        async function checkIfLoggedIn(){
            const sessionFront = AuthHelpers.IsLoggedIn();
            const sessionBack = await Api.Session();

            if (sessionBack && sessionFront){
                nav(Paths.main);
                return;
            }

            if (sessionBack && !sessionFront)
                Api.Logout(); 
        }

        checkIfLoggedIn();
    }, [nav])

    return <>
        <header>
            
        </header>
        <main className="app container">
            <UPSToastContainer/>
            {page}
        </main>
    </>
}