import { ErrorMessage } from "formik"

export type Option = {
    value: any,
    label: string
}

export const ValidationMessage = ({ fieldName }: { fieldName: string }) => {
    return <ErrorMessage name={fieldName} className="error-msg" component="div" />
}