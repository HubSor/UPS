import { FieldArray, Form, Formik } from "formik"
import { SalePathFormProps } from "../../pages/SaleMainPage"
import { Button } from "react-bootstrap"
import React from "react"
import { ParameterTypeEnum, SalePathParameterDto } from "../../api/Dtos"
import { CheckboxParameter, DecimalParameter, IntegerParameter, ParameterProps, SelectParameter, TextAreaParameter, TextParameter } from "../../helpers/ParameterHelpers"

type FillParameterValuesProps = SalePathFormProps

type FillParameterValuesFormValues = {
    params: SalePathParameterDto[]
}

export const FillParameterValuesForm = ({ state, dispatch }: FillParameterValuesProps) => {
    const initialValues: FillParameterValuesFormValues = {
        params: state.product?.parameters.map(p => ({ ...p, answer: undefined })) ?? [],
    }

    return <div>
        <h3>Uzupełnij parametry sprzedaży</h3>
        <br/>
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                dispatch({ type: 'filledParameters', paramterValues: v.params })
                fh.setSubmitting(false);
            }}
        >
            {({ isSubmitting }) => {
                return <Form>
                    <FieldArray name="params" render={(fh) => <div>
                        {state.product?.parameters.map((p, idx) => {
                            const props: ParameterProps = { param: p, fieldName: `params.${idx}.answer`}
                            return <React.Fragment key={p.id}>
                                {p.type === ParameterTypeEnum.Text && <TextParameter {...props} />}
                                {p.type === ParameterTypeEnum.Integer && <IntegerParameter {...props} />}
                                {p.type === ParameterTypeEnum.Decimal && <DecimalParameter {...props} />}
                                {p.type === ParameterTypeEnum.Select && <SelectParameter {...props} />}
                                {p.type === ParameterTypeEnum.Checkbox && <CheckboxParameter {...props} />}
                                {p.type === ParameterTypeEnum.TextArea && <TextAreaParameter {...props} />}
                            </React.Fragment>
                        })}
                    </div>}/>
                    <div>
                        <Button type="submit" disabled={isSubmitting}>
                            Zapisz
                        </Button>
                    </div>
                </Form>}
            }
        </Formik>
    </div>
}