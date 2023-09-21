import { useNavigate } from "react-router-dom";
import { Api } from "../api/Api";
import { AuthHelpers } from "../helpers/AuthHelper";
import { Paths } from "../App";

export default function Header(){
    const nav = useNavigate();

    return <div className="header">
        <span>
            <h1>Header</h1>
            Logged in as {AuthHelpers.GetUserData()?.username ?? "nobody"}
            {AuthHelpers.IsLoggedIn() && <button type="button"
                onClick={() => {
                    AuthHelpers.ClearAllData();
                    Api.Logout();
                    nav(Paths.login)
                }}
            >
                Wyloguj
            </button>}
        </span>
    </div>
}