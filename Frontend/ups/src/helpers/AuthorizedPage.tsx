import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { AuthHelpers } from "./AuthHelper";
import { Paths } from "../App";
import Header from "../components/Header";
import { Api } from "../api/Api";

type AuththorizedPageProps = {
    page: JSX.Element
}

export function AuthorizedPage({ page }: AuththorizedPageProps) {
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
        }

        checkIfLoggedIn();
    }, [nav])

    return <>
        <header>
            <Header />
        </header>
        <main className='app container'>
            {page}   
        </main>
    </>
}