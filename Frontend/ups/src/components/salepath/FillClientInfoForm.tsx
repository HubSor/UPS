import { Form, Formik } from "formik"
import { SalePathFormProps } from "../../pages/SaleMainPage"
import { UpsertClientRequest } from "../../api/ApiRequests"
import { object } from "yup"
import { Api } from "../../api/Api"
import { ApiResponse, UpsertClientResponse } from "../../api/ApiResponses"
import { toastError, toastInfo } from "../../helpers/ToastHelpers"
import { SeparateErrors, TypeInputGroup, ValidationMessage } from "../../helpers/FormHelpers"
import { useEffect } from "react"
import { Button } from "react-bootstrap"

type FillClientInfoProps = SalePathFormProps & {
    
}

const fillClientInfoSchema = object<UpsertClientRequest>().shape({
    
})

export const FillClientInfoForm = ({ state, dispatch }: FillClientInfoProps) => {
    const initialValues: UpsertClientRequest = {
        isCompany: false,        
    }

    return <div>
        <h3>Uzupełnij dane klienta</h3>
        <br/>
        <Formik
            initialValues={initialValues}
            validationSchema={fillClientInfoSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<UpsertClientResponse>) => {
                    if (res.success && res.data) {
                        toastInfo('Zapisano klienta')
                        dispatch({ type: 'setClient', clientId: res.data.clientId })
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastError('Nie udało się zapisać klienta')
                }

                Api.UpsertClient(v).then(handleApiResponse)
            }}
        >
            {function FormInner({ isSubmitting, values, setFieldValue }) {
                useEffect(() => {
                    

                    if (!!values.clientId) {
                        if (values.isCompany && (!!values.regon || !!values.nip))
                            Api.FindCompanyClient({ identifier: values.regon ?? values.nip ?? "" });
                        if (!values.isCompany && !!values.pesel)
                            Api.FindCompanyClient({ identifier: values.pesel });
                    }
                }, [values.clientId, values.isCompany, values.nip, values.pesel, values.regon]);

                return <Form>
                    <ValidationMessage fieldName="clientId" />
                    {!values.isCompany ? <>
                        <TypeInputGroup name="firstName" label="Imię" type="text" />
                        <TypeInputGroup name="lastName" label="Nazwisko" type="text" />
                        <TypeInputGroup name="pesel" label="PESEL" type="text" />
                    </> : <>
                        <TypeInputGroup name="companyName" label="Nazwa firmy" type="text" />
                        <TypeInputGroup name="regon" label="REGON" type="text" />
                        <TypeInputGroup name="nip" label="NIP" type="text" />
                    </>}

                    <TypeInputGroup name="phoneNumber" label="Numer telefonu" type="text" />
                    <TypeInputGroup name="email" label="Adres E-mail" type="text" />
                    <TypeInputGroup name="nip" label="NIP" type="text" />
                    
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