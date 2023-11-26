import { NavigateFunction, useNavigate } from "react-router-dom"
import { Paths } from "../App";
import { showClients, showSaleHistory, showSales, showUsers } from "../helpers/AuthHelper";

export default function MainPage() {
    const nav = useNavigate();

    return <>
        <h4>Uniwersalna Platforma Sprzedażowa</h4>
        <br/>
        <br/>
        <div className="container">
            <div className="row row-cols-3 g-3 g-lg-3">
                {showSales() && <MainPageCard title="Ścieżka Sprzedaży" url={Paths.salePath} nav={nav}/>}
                <MainPageCard title="Produkty" url={Paths.products} nav={nav} />
                <MainPageCard title="Podprodukty" url={Paths.subProducts} nav={nav} />
                {showUsers() && <MainPageCard title="Uzytkownicy" url={Paths.users} nav={nav} />}
                {showSaleHistory() && <MainPageCard title="Transakcje" url={Paths.sales} nav={nav} />}
                {showClients() && <MainPageCard title="Klienci" url={Paths.clients} nav={nav} />}
            </div>
        </div>
    </>
}

type MainPageCardProps = {
    title: string,
    url: string,
    nav: NavigateFunction
}

const MainPageCard = ({ title, url, nav }: MainPageCardProps) => <div className="col">
    <div className="card p-3">
        <div className="card-body" onClick={() =>  nav(url)}>
            <h5 className="card-title">{title}</h5>
            <p className="card-text">Kliknij aby przejść</p>
        </div>
    </div>
</div>