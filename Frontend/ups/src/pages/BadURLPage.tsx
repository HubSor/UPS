import Header from "../components/Header";
import { AuthHelpers } from "../helpers/AuthHelper";

export default function BadURLPage() {
    return <>
        <header>
            {AuthHelpers.IsLoggedIn() && <Header />}
        </header>
        <main className="app container">
            <br/>
            <h5>
                Niepoprawny URL
            </h5>
        </main>
    </>
}
