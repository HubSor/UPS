import { Form } from "react-bootstrap";
import { ParameterDto } from "../api/Dtos";
import { Field } from "formik";
import { ValidationMessage } from "./FormHelpers";
import React from "react";

function getParamDisplay(p: ParameterDto) {
    return p.name + (p.required ? "*" : " (opcjonalny)") 
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
        <Field type='text' name={fieldName} className="form-control" />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function IntegerParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field type='number' name={fieldName} className="form-control" />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function DecimalParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field type='number' name={fieldName} className="form-control" />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function TextAreaParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field as={'textarea'} rows={4} values={undefined} className="form-control" name={fieldName} />
        <ValidationMessage fieldName={fieldName} />
    </Form.Group>
}

export function SelectParameter({ param, fieldName }: ParameterProps) {
    return <Form.Group className="mb-3">
        <Form.Label>
            {getParamDisplay(param)}
        </Form.Label>
        <Field as="select" name={fieldName} className="form-control">
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
        <Field type="checkbox" name={fieldName} className="form-check-input" />
        &nbsp;
        <Form.Label className="form-check-label">
            {getParamDisplay(param)}
        </Form.Label>
        <ValidationMessage fieldName={fieldName} />
    </Form.Check>
}