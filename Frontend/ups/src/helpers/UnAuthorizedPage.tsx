import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { AuthHelpers } from "./AuthHelper";
import { Paths } from "../App";

type UnAuththorizedPageProps = {
    page: JSX.Element
}

export function UnAuthorizedPage({ page }: UnAuththorizedPageProps) {
    const nav = useNavigate();

    useEffect(() => {
        if (AuthHelpers.IsLoggedIn())
            nav(Paths.main);
    }, [nav])

    return <>
        {page}
    </>
}