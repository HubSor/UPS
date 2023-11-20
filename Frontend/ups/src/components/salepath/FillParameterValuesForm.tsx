import { FieldArray, Form as FForm, Formik } from "formik"
import { SalePathFormProps, getSelectedSubProducts } from "../../pages/SaleMainPage"
import { Button, Form } from "react-bootstrap"
import React from "react"
import { SalePathParameterDto } from "../../api/Dtos"
import { ParameterProps, ParameterSwitch } from "../../helpers/ParameterHelpers"

type FillParameterValuesProps = SalePathFormProps

type FillParameterValuesFormValues = {
    productParams: SalePathParameterDto[]
    subProductParams: SalePathParameterDto[]
}

export const FillParameterValuesForm = ({ state, dispatch }: FillParameterValuesProps) => {
    const paramsFromProduct: SalePathParameterDto[] = state.product?.parameters.map(p => ({ ...p, answer: undefined })) ?? [];
    const paramsFormSubProducts: SalePathParameterDto[] = getSelectedSubProducts(state)
        .flatMap(sp => sp.parameters.map(p => ({ ...p, answer: undefined }))) ?? []

    const initialValues: FillParameterValuesFormValues = {
        productParams: paramsFromProduct,
        subProductParams: paramsFormSubProducts
    }

    return <div>
        <h3>Uzupełnij parametry sprzedaży</h3>
        <br/>
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                dispatch({ type: 'filledParameters', productValues: v.productParams, subProductValues: v.subProductParams })
                fh.setSubmitting(false);
            }}
        >
            {({ isSubmitting }) => {
                const selectedSubproducts = getSelectedSubProducts(state);

                return <FForm>
                    <FieldArray name="params" render={(fh) => <div>
                        <Form.Label className="align-left">
                            Parametry produktu
                        </Form.Label>
                        {state.product?.parameters.map((p, idx) => {
                            const props: ParameterProps = { param: p, fieldName: `params.${idx}.answer`}
                            return <React.Fragment key={p.id}>
                                <ParameterSwitch {...props}/>
                            </React.Fragment>
                        })}
                        {selectedSubproducts.length > 0 && <>
                            <Form.Label className="align-left">
                                Parametry podproduktów
                            </Form.Label>
                            {selectedSubproducts.flatMap(sp => sp.parameters.map((p ,idx) => {
                                const props: ParameterProps = { param: p, fieldName: `params.${idx}.answer` }
                                return <React.Fragment key={p.id}>
                                    <ParameterSwitch {...props} />
                                </React.Fragment> 
                            }))}
                        </>}
                    </div>}/>
                    <div>
                        <Button type="submit" disabled={isSubmitting}>
                            Zapisz
                        </Button>
                    </div>
                </FForm>}
            }
        </Formik>
    </div>
}