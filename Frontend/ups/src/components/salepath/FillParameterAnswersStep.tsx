import { FieldArray, Form as FForm, Formik } from "formik"
import { SalePathStepProps, getSelectedSubProducts } from "../../pages/SalePathPage"
import { Button } from "react-bootstrap"
import React from "react"
import { AnsweredParameterDto } from "../../api/Dtos"
import { ParameterProps, ParameterSwitch } from "../../helpers/ParameterHelpers"
import { toastInfo } from "../../helpers/ToastHelpers"

type FillParameterAnswersProps = SalePathStepProps

type FillParameterAnswersFormValues = {
    productParams: AnsweredParameterDto[]
    subProductParams: AnsweredParameterDto[]
}

export const FillParameterAnswersStep = ({ state, dispatch }: FillParameterAnswersProps) => {
    const paramsFromProduct: AnsweredParameterDto[] = !!state.productAnswers && state.productAnswers.length > 0 ?
        state.productAnswers :
        state.product?.parameters.map(p => ({ ...p, answer: undefined })) ?? [];
    const paramsFormSubProducts: AnsweredParameterDto[] = !!state.subProductAnswers && state.subProductAnswers.length > 0 ?
        state.subProductAnswers :
        getSelectedSubProducts(state).flatMap(sp => sp.parameters.map(p => ({ ...p, answer: undefined, subProductId: sp.id }))) ?? []

    const initialValues: FillParameterAnswersFormValues = {
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
                dispatch({ type: 'nextStep' })
                toastInfo('Zapisano parametry sprzedaży')
                fh.setSubmitting(false);
            }}
        >
            {({ isSubmitting, values }) => {
                const selectedSubproducts = getSelectedSubProducts(state);

                return <FForm>
                    <FieldArray name="productParams" render={(fh) => <div>
                        <h5 className="align-left">
                            Parametry produktu
                        </h5>
                        <div className="align-left">
                            {state.product?.name}
                        </div>
                        {state.product?.parameters.map((p, idx) => {
                            const props: ParameterProps = { param: p, fieldName: `productParams.${idx}.answer`}
                            return <React.Fragment key={p.id}>
                                <ParameterSwitch {...props}/>
                            </React.Fragment>
                        })}
                        
                    </div>}/>
                    <FieldArray name="subProductParams" render={(fh) => <div>
                        {selectedSubproducts.length > 0 && <>
                            <h5 className="align-left">
                                Parametry podproduktów
                            </h5>
                            {selectedSubproducts.flatMap(sp => {
                                return <React.Fragment key={sp.id}>
                                    <div className="align-left">
                                        {sp.name}
                                    </div>
                                    {sp.parameters.map(p => {
                                        const idx = values.subProductParams.findIndex(x => x.id === p.id)
                                        const props: ParameterProps = { param: p, fieldName: `subProductParams.${idx}.answer` }
                                        return <React.Fragment key={p.id}>
                                            <ParameterSwitch {...props} />
                                        </React.Fragment>
                                    })}
                                </React.Fragment>
                            })}
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