import Footer from './components/Footer';
import { Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import MainPage from './pages/MainPage';
import { AuthorizedPage } from './helpers/AuthorizedPage';
import { UnAuthorizedPage } from './helpers/UnAuthorizedPage';
import UserMainPage from './pages/UserMainPage';
import 'bootstrap/dist/css/bootstrap.css';
import 'react-toastify/dist/ReactToastify.css';
import './style/App.css';
import ProductMainPage from './pages/ProductMainPage';
import SubSubProductMainPage from './pages/SubProductMainPage';

export const Paths = {
    login: "/login",
    main: "/",
    users: "/users",
    products: "/products",
    subProducts: "/subproducts"
}

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route>
            <Route path={Paths.login} element={<UnAuthorizedPage page={<LoginPage/>} />} />
            <Route path={Paths.main} element={<AuthorizedPage page={<MainPage/>} />} />
            <Route path={Paths.users} element={<AuthorizedPage page={<UserMainPage />} />} />
            <Route path={Paths.products} element={<AuthorizedPage page={<ProductMainPage />} />} />
            <Route path={Paths.subProducts} element={<AuthorizedPage page={<SubSubProductMainPage />} />} />
        </Route>
    )
)
export default function App() { 

    return <>
        <RouterProvider router={router} />
        <footer>
            <Footer />
        </footer>
    </>;
}
