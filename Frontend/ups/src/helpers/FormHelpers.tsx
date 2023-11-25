import { ErrorMessage, Field } from "formik"
import { ResultPaginationDto } from "../api/Dtos"
import { Form } from "react-bootstrap"
import React from "react"

export type Option = {
    value: any,
    label: string
}

export const defaultPagination: ResultPaginationDto = {
    pageSize: 10,
    pageIndex: 0,
    totalCount: 0,
    totalPages: 1,
    count: 0
}

export const ValidationMessage = ({ fieldName }: { fieldName: string }) => {
    return <ErrorMessage name={fieldName} render={msg => {
        return <div className="error-msg">
            {msg.split('\n').map((m, idx) => <React.Fragment key={idx}>
                {m}
                <br/>
            </React.Fragment>)}
        </div>
    }}/>
}

export const NoFormikValidationMessage = ({ msg }: { msg?: string }) => {
    if (!msg)
        return <></>

    return <div className="error-msg m-2">
        {msg.split('\n').map((m, idx) => <React.Fragment key={idx}>
            {m}
            <br />
        </React.Fragment>)}
    </div>
}

export const SeparateErrors = (errors: { [key: string]: string[] }) => {
    let newObj: { [k: string]: string } = {};
    Object.keys(errors).forEach(k => newObj[k] =  errors[k].join('\n'));
    return newObj;
}

export const JoinErrors = (errors: { [key: string]: string[] }) => {
    let newStr = "";
    Object.keys(errors).forEach(k => newStr += errors[k].join('\n') + '\n');
    return newStr;
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
    rows?: number
}

export const InlineTextInputGroup = (props: InputGroupProps) => {
    return <div className="form-group row mb-4">
        <label className="col-sm-2 col-form-label">{props.label}</label>
        <div className="col">
            <Field type="text" name={props.name} className="form-control"/>
            <ValidationMessage fieldName={props.name} />
        </div>
    </div>
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

export function CheckboxInputGroup(props: InputGroupProps) {
    return <Form.Check className="mb-3">
        <Field type="checkbox" name={props.name} className="form-check-input" />
        &nbsp;
        <Form.Label className="form-check-label">
            {props.label}
        </Form.Label>
        <ValidationMessage fieldName={props.name} />
    </Form.Check>
}

export function AsInputGroup(props: AsInputGroupProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {props.label}
        </Form.Label>
        {props.as === 'textarea' ?
            <TestAreaField {...props} /> :
            <Field as={props.as} name={props.name} className="form-control">
                {!!props.options && props.options.map(o => {
                    return <React.Fragment key={o.value}>
                        <option value={o.value}>{o.label}</option>
                    </React.Fragment>
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

    const disableLeft = props.currentIndex <= 0;
    const disableRight = props.currentIndex >= props.maxIndex;

    return <nav className="m-2">
        <ul className="pagination justify-content-center">
            <li className="page-item">
                <button disabled={disableLeft} type="button" className={"page-link " + (disableLeft ? "disabled" : "")} onClick={prev}>
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
                <button disabled={disableRight} className={"page-link " + (disableRight ? "disabled" : "")} onClick={next}>
                    {">"}
                </button>
            </li>
        </ul>
    </nav>;
}
