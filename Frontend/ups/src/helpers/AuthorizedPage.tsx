import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { AuthHelpers } from "./AuthHelper";
import { Paths } from "../App";
import Header from "../components/Header";

type AuththorizedPageProps = {
    page: JSX.Element
}

export function AuthorizedPage({ page }: AuththorizedPageProps) {
    const nav = useNavigate();

    useEffect(() => {
        if (!AuthHelpers.IsLoggedIn())
            nav(Paths.login);
    }, [nav])

    return <>
        <Header />
        {page}   
    </>
}