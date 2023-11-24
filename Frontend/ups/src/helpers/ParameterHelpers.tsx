import { Form } from "react-bootstrap";
import { ParameterDto, ParameterTypeEnum } from "../api/Dtos";
import { Field } from "formik";
import { ValidationMessage } from "./FormHelpers";
import React from "react";

function getParamDisplay(p: ParameterDto) {
    return p.name + (p.required ? "*" : " (opcjonalny)") 
}

const baseParameterValidator = (p: ParameterDto, validators: ((v: any) => string | undefined)[]) => {
    return (value: any) => {
        if (p.required)
            validators.push(validateRequired);
        
        for (const validator of validators){
            const result = validator(value)
            if (result)
                return result;
        }
    }
}

const validateRequired = (value: any) => {
    return !value ? "Pole wymagane" : undefined;
}

const validateText = (value: any) => {
    if (value?.length > 256)
        return "Zbyt długa wartość pola";
}

const validateTextArea = (value: any) => {
    if (value?.length > 1024)
        return "Zbyt długa wartość pola";
}

const validateInteger = (value: any) => {
    if (!value)
        return
    if (!/^\d+$/.test(value))
        return "Niepoprawna liczba całkowita";
    if (+value > 1_000_000_000)
        return "Zbyt duża liczba";
}

const validateDecimal = (value: any) => {
    if (!value)
        return
    if (!/^-{0,1}\d*\.{0,1}\d+$/.test(value))
        return "Niepoprawna liczba dziesiętna";
    if (+value > 1_000_000_000)
        return "Zbyt duża liczba";
}

export type ParameterProps = {
    param: ParameterDto,
    fieldName: string,
}

export function TextParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field validate={baseParameterValidator(param, [validateText])}
            type='text' name={fieldName} className="form-control" />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function IntegerParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field validate={baseParameterValidator(param, [validateInteger])}
            type='number' name={fieldName} className="form-control" />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function DecimalParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field type='number' validate={baseParameterValidator(param, [validateDecimal])}
            name={fieldName} className="form-control" step={"0,01"} />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function TextAreaParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field as={'textarea'} rows={4} validate={baseParameterValidator(param, [validateTextArea])}
            values={undefined} className="form-control" name={fieldName} />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function SelectParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field as="select" name={fieldName} className="form-control" validate={baseParameterValidator(param, [])}>
            <option value={undefined}>---</option>
            {!!param.options && param.options.map(o => {
                return <React.Fragment key={o.value}>
                    <option value={o.value}>{o.value}</option>
                </React.Fragment>
            })}
        </Field>
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function CheckboxParameter({ param, fieldName }: ParameterProps) {
    return <Form.Check className="mb-3">
        <Field type="checkbox" name={fieldName} className="form-check-input"
            validate={baseParameterValidator(param, [])} />
        &nbsp;
        <Form.Label className="form-check-label">
            {getParamDisplay(param)}
        </Form.Label>
        <ValidationMessage fieldName={fieldName} />
    </Form.Check>
}

export function ParameterSwitch(props: ParameterProps){
    return <>
        {props.param.type === ParameterTypeEnum.Text && <TextParameter {...props} />}
        {props.param.type === ParameterTypeEnum.Integer && <IntegerParameter {...props} />}
        {props.param.type === ParameterTypeEnum.Decimal && <DecimalParameter {...props} />}
        {props.param.type === ParameterTypeEnum.Select && <SelectParameter {...props} />}
        {props.param.type === ParameterTypeEnum.Checkbox && <CheckboxParameter {...props} />}
        {props.param.type === ParameterTypeEnum.TextArea && <TextAreaParameter {...props} />}
    </>
}