import Footer from './components/Footer';
import { Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import MainPage from './pages/MainPage';
import { AuthorizedPage } from './helpers/AuthorizedPage';
import { UnAuthorizedPage } from './helpers/UnAuthorizedPage';
import 'bootstrap/dist/css/bootstrap.css';
import 'react-toastify/dist/ReactToastify.css';
import './style/App.css';
import ProductPage from './pages/ProductPage';
import SubProductPage from './pages/SubProductPage';
import SalePathPage from './pages/SalePathPage';
import UserListPage from './pages/UserListPage';
import ProductListPage from './pages/ProductListPage';
import SubProductListPage from './pages/SubProductListPage';
import SaleListPage from './pages/SaleListPage';

export const Paths = {
    login: "/login",
    main: "/",
    users: "/users",
    products: "/products",
    product: "/product/:id",
    subProducts: "/subproducts",
    subProduct: "/subproduct/:id",
    salePath: "/sale",
    sales: "/sales",
}

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route>
            <Route path={Paths.login} element={<UnAuthorizedPage page={<LoginPage/>} />} />
            <Route path={Paths.main} element={<AuthorizedPage page={<MainPage/>} />} />
            <Route path={Paths.users} element={<AuthorizedPage page={<UserListPage />} />} />
            <Route path={Paths.products} element={<AuthorizedPage page={<ProductListPage />} />} />
            <Route path={Paths.product} element={<AuthorizedPage page={<ProductPage />} />} />
            <Route path={Paths.subProducts} element={<AuthorizedPage page={<SubProductListPage />} />} />
            <Route path={Paths.subProduct} element={<AuthorizedPage page={<SubProductPage />} />} />
            <Route path={Paths.salePath} element={<AuthorizedPage page={<SalePathPage />} />} />
            <Route path={Paths.sales} element={<AuthorizedPage page={<SaleListPage />} />} />
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
