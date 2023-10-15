import { ErrorMessage, Field } from "formik"
import { RoleEnum } from "../api/Dtos"
import { Form } from "react-bootstrap"

export type Option = {
    value: any,
    label: string
}

export const ValidationMessage = ({ fieldName }: { fieldName: string }) => {
    return <ErrorMessage name={fieldName} render={msg => <div className="error-msg">
        {msg.split('\n').map(m => <>{m}<br/></>)}
    </div>}/>
}

export const SeparateErrors = (errors: { [key: string]: string[] }) => {
    let newObj: { [k: string]: string } = {};
    Object.keys(errors).forEach(k => newObj[k] =  errors[k].join('\n'));
    return newObj;
}

export const RoleEnumDisplayName = (role: RoleEnum) => {
    switch (role) {
        case RoleEnum.Administrator:
            return "Administrator";
        case RoleEnum.UserManager:
            return "Zarządca użytkowników";
        case RoleEnum.Seller:
            return "Sprzedawca";
    }
}

type InputGroupProps = {
    name: string,
    label: string,
    type: string
}

export function InputGroup(props: InputGroupProps){
    return <Form.Group className="mb-3">
        <Form.Label>
            {props.label}
        </Form.Label>
        <Field type={props.type} name={props.name} className="form-control"/>
        <ValidationMessage fieldName={props.name}/>
    </Form.Group>
}