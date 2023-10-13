import { Field, Form, Formik } from "formik";
import { Api } from "../api/Api";
import { useNavigate } from "react-router-dom";
import { Paths } from "../App";
import { AuthHelpers } from "../helpers/AuthHelper";
import { LoginRequest } from "../api/ApiRequests";
import { object, string } from "yup";
import { ValidationMessage } from "../helpers/FormHelpers";

const initialValues: LoginRequest = {
    username: "",
    password: ""
}

const loginValidationSchema = object<LoginRequest>().shape({
    username: string()
        .max(64, "Zbyt długi login")
        .min(4, "Zbyt krótki login")
        .required("Należy podać login"),
    password: string()
        .max(128, "Zbyt długie hasło")
        .required("Należy podać hasło")
})

export default function LoginForm(){
    const nav = useNavigate();

    return <div className="login">
        <Formik
            validationSchema={loginValidationSchema}
            initialValues={initialValues}
            onSubmit={async (values, fh) => {
                await Api.Login(values).then(res => {
                    if (res.success && res.data){
                        nav(Paths.main)
                        AuthHelpers.StoreUserData(res.data.userDto);
                    }
                    else {
                        fh.setErrors(res.errors);
                        fh.setFieldValue('password', '', false);
                    }
                })
            }}
        >
            {() => <Form>
                <div className="form-group row">
                    <label className="col-sm-1 col-form-label">Login</label>
                    <div className="col-sm-3">
                        <Field type="text" name="username" className="form-control"/>
                        <ValidationMessage fieldName="username" />
                    </div>
                </div>
                <br/>
                <div className="form-group row">
                    <label className="col-sm-1 col-form-label">Hasło</label>
                    <div className="col-sm-3">
                        <Field type="password" name="password" className="form-control"/>
                        <ValidationMessage fieldName="password" />
                    </div>
                </div>
                <br/>
                <button className="btn btn-primary" type="submit">Zaloguj</button>
            </Form>}
        </Formik>
    </div>
}