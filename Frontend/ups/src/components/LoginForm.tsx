import { Form, Formik } from "formik";
import { Api } from "../api/Api";

export default function LoginForm(){
    return <div>
        <Formik
            initialValues={{ username: "", password: ""}}
            onSubmit={(values) => {
                Api.Login(values).then(res => {
                    
                })
            }}
        >
            {() => <Form>
                
            </Form>}
        </Formik>
    </div>
}