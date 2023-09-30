import './style/App.css';
import Footer from './components/Footer';
import { Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import MainPage from './pages/MainPage';
import { AuthorizedPage } from './helpers/AuthorizedPage';
import { UnAuthorizedPage } from './helpers/UnAuthorizedPage';
import UserMainPage from './pages/UserMainPage';
import 'bootstrap/dist/css/bootstrap.css';

export const Paths = {
    login: "/login",
    main: "/",
    users: "/users"
}

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route>
            <Route path={Paths.login} element={<UnAuthorizedPage page={<LoginPage/>} />} />
            <Route path={Paths.main} element={<AuthorizedPage page={<MainPage/>} />} />
            <Route path={Paths.users} element={<AuthorizedPage page={<UserMainPage />} />} />
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
