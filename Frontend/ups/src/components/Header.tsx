import { useNavigate } from "react-router-dom";
import { Api } from "../api/Api";
import { AuthHelpers, showClients, showSaleHistory, showSales, showUsers } from "../helpers/AuthHelper";
import { Paths } from "../App";

export default function Header(){
    const nav = useNavigate();

    return <nav className="navbar navbar-expand-md fixed-top navbar-dark bg-dark">
        <a href={Paths.main} className="navbar-title">UPS</a>
        <div className="collapse navbar-collapse">
            <ul className="navbar-nav mr-auto">
                {showSales() && <li className="nav-item">
                    <a className="nav-link" href={Paths.salePath}>Sprzedaż</a>
                </li>}
                <li className="nav-item">
                    <a className="nav-link" href={Paths.products}>Produkty</a>
                </li>
                <li className="nav-item">
                    <a className="nav-link" href={Paths.subProducts}>Podprodukty</a>
                </li>
                {showUsers() && <li className="nav-item">
                    <a className="nav-link" href={Paths.users}>Użytkownicy</a>
                </li>}
                {showSaleHistory() && <li className="nav-item">
                    <a className="nav-link" href={Paths.sales}>Transakcje</a>
                </li>}
                {showClients() && <li className="nav-item">
                    <a className="nav-link" href={Paths.clients}>Klienci</a>
                </li>}
            </ul>
        </div>
        <div className="navbar-login">
            <span className="username">
                {AuthHelpers.GetUserData()?.username ? "Zalogowano jako " + AuthHelpers.GetUserData()?.username : ""}
            </span>
            {" "}
            <span className="m-1">
                {AuthHelpers.IsLoggedIn() && <button type="button" className="btn btn-outline-light btn-sm"
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
    </nav>
}