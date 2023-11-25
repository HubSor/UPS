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
import ClientListPage from './pages/ClientListPage';
import SalePage from './pages/SalePage';
import { RoleEnum } from './api/Dtos';
import BadURLPage from './pages/BadURLPage';

export const Paths = {
    login: "/login",
    main: "/",
    users: "/users",
    products: "/products",
    product: "/product/:id",
    subProducts: "/subproducts",
    subProduct: "/subproduct/:id",
    salePath: "/salepath",
    sales: "/sales",
    sale: "/sale/:id",
    clients: "/clients",
}

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route>
            <Route path={Paths.login} element={<UnAuthorizedPage page={<LoginPage/>} />} />
            <Route path={Paths.main} element={<AuthorizedPage page={<MainPage/>} requiredRoles />} />
            <Route path={Paths.users} element={<AuthorizedPage requiredRoles={[RoleEnum.UserManager]} page={<UserListPage />} />} />
            <Route path={Paths.products} element={<AuthorizedPage requiredRoles page={<ProductListPage />} />} />
            <Route path={Paths.product} element={<AuthorizedPage requiredRoles={[RoleEnum.ProductManager]} page={<ProductPage />} />} />
            <Route path={Paths.subProducts} element={<AuthorizedPage requiredRoles page={<SubProductListPage />} />} />
            <Route path={Paths.subProduct} element={<AuthorizedPage requiredRoles={[RoleEnum.ProductManager]} page={<SubProductPage />} />} />
            <Route path={Paths.salePath} element={<AuthorizedPage requiredRoles={[RoleEnum.Seller, RoleEnum.SaleManager]} page={<SalePathPage />} />} />
            <Route path={Paths.sales} element={<AuthorizedPage requiredRoles={[RoleEnum.SaleManager]} page={<SaleListPage />} />} />
            <Route path={Paths.sale} element={<AuthorizedPage requiredRoles={[RoleEnum.SaleManager]} page={<SalePage />} />} />
            <Route path={Paths.clients} element={<AuthorizedPage requiredRoles={[RoleEnum.ClientManager]} page={<ClientListPage />} />} />
            <Route path='*' element={<BadURLPage />} />
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
