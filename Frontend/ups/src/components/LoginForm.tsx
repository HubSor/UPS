import { ErrorMessage, Field, Form, Formik } from "formik";
import { Api } from "../api/Api";
import { useNavigate } from "react-router-dom";
import { Paths } from "../App";
import { AuthHelpers } from "../helpers/AuthHelper";

export default function LoginForm(){
    const nav = useNavigate();

    return <div>
        <Formik
            initialValues={{ username: "", password: ""}}
            onSubmit={(values, fh) => {
                Api.Login(values).then(res => {
                    if (res.success && res.data){
                        nav(Paths.main)
                        AuthHelpers.StoreUserData(res.data.userDto);
                    }
                    else {
                        fh.setErrors(res.errors);
                        fh.setFieldValue('password', '');
                    }
                })
            }}
        >
            {() => <Form>
                <div>
                    <label>Login</label>
                    <Field type="text" name="username"/>
                    <ErrorMessage name="username" component="div"/>
                </div>
                <div>
                    <label>Hasło</label>
                    <Field type="password" name="password"/>
                    <ErrorMessage name="password" component="div" />
                </div>
                <nav>
                    <button type="submit">Zaloguj</button>
                </nav>
            </Form>}
        </Formik>
    </div>
}