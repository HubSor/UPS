import { ErrorMessage, Field } from "formik"
import { ProductStatusEnum, RoleEnum } from "../api/Dtos"
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
        case RoleEnum.ProductManager:
            return "Zarządca produktów";
    }
}

export const ProductStatusEnumDisplayName = (role: ProductStatusEnum) => {
    switch (role) {
        case ProductStatusEnum.NotOffered:
            return "Nieoferowany";
        case ProductStatusEnum.Withdrawn:
            return "Wycofany";
        case ProductStatusEnum.Offered:
            return "Oferowany";
    }
}

type InputGroupProps = {
    name: string,
    label: string,
}

type TypeInputGroupProps = InputGroupProps & {
    type?: string,
}

type AsInputGroupProps = InputGroupProps & {
    options?: Option[],
    as?: string,
}

export function TypeInputGroup(props: TypeInputGroupProps){
    return <Form.Group className="mb-3">
        <Form.Label>
            {props.label}
        </Form.Label>
        <Field type={props.type} name={props.name} className="form-control"/>
        <ValidationMessage fieldName={props.name}/>
    </Form.Group>
}

export function AsInputGroup(props: AsInputGroupProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {props.label}
        </Form.Label>
        {props.as === 'textarea' ?
            <TestAreaField {...props} /> :
            <Field as={props.as} name={props.name} value={undefined} className="form-control">
                {!!props.options && props.options.map(o => {
                    return <>
                        <option value={o.value} key={o.value}>{o.label}</option>
                    </>
                })}
            </Field>
        }
        <ValidationMessage fieldName={props.name} />
    </Form.Group>
}

const TestAreaField = (props: AsInputGroupProps) =>
    <Field as={props.as} values={undefined} className="form-control" name={props.name} />

export type PaginationBarProps = {
    onNext: (next: number) => void
    onPrev: (prev: number) => void
    currentIndex: number,
    maxIndex: number
}

export function PaginationBar(props: PaginationBarProps) {
    const next = () => props.onNext(props.currentIndex + 1);
    const prev = () => props.onNext(props.currentIndex - 1);

    return <nav className="m-2">
        <ul className="pagination justify-content-center">
            <li className="page-item">
                <button disabled={props.currentIndex <= 0} type="button" className="page-link" onClick={prev}>
                    {"<"}
                </button>
            </li>
            {props.currentIndex > 0 && <li className="page-item">
                <button className="page-link" onClick={prev}>
                    {props.currentIndex}
                </button>
            </li>}
            <li className="page-item active"><button className="page-link">{props.currentIndex + 1}</button></li>
            {props.currentIndex < props.maxIndex && <li className="page-item">
                <button className="page-link" onClick={next}>
                    {props.currentIndex + 2}
                </button>
            </li>}
            <li className="page-item">
                <button disabled={props.currentIndex >= props.maxIndex} className="page-link" onClick={next}>
                    {">"}
                </button>
            </li>
        </ul>
    </nav>;
}
