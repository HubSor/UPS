import { Form, Formik } from "formik"
import { SalePathFormProps } from "../../pages/SaleMainPage"
import { UpsertClientRequest } from "../../api/ApiRequests"
import { object } from "yup"
import { Api } from "../../api/Api"
import { ApiResponse, UpsertClientResponse } from "../../api/ApiResponses"
import { toastError, toastInfo } from "../../helpers/ToastHelpers"
import { CheckboxInputGroup, SeparateErrors, TypeInputGroup, ValidationMessage } from "../../helpers/FormHelpers"
import { useCallback, useEffect } from "react"
import { Button } from "react-bootstrap"
import debounce from 'lodash.debounce';

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
            {function FormInner({ isSubmitting, values, setFieldValue, isValid }) {
                const debouncedFindClient = useCallback(() => {
                    return debounce(() => {
                        if (values.isCompany && (!!values.regon || !!values.nip)) {
                            Api.FindCompanyClient({ identifier: values.regon ?? values.nip ?? "" }).then(res => {
                                if (res.data && res.success) {
                                    setFieldValue('regon', res.data.companyClient.regon)
                                    setFieldValue('nip', res.data.companyClient.nip)
                                    setFieldValue('companyName', res.data.companyClient.companyName)
                                    setFieldValue('phoneNumber', res.data.companyClient.phoneNumber)
                                    setFieldValue('email', res.data.companyClient.email)
                                    setFieldValue('clientId', res.data.companyClient.id)
                                    toastInfo("Wczytano firmę " + res.data.companyClient.companyName)
                                }
                            });
                        }
                        if (!values.isCompany && !!values.pesel) {
                            Api.FindPersonClient({ identifier: values.pesel }).then(res => {
                                if (res.data && res.success) {
                                    setFieldValue('pesel', res.data.personClient.pesel)
                                    setFieldValue('firstName', res.data.personClient.firstName)
                                    setFieldValue('lastName', res.data.personClient.lastName)
                                    setFieldValue('phoneNumber', res.data.personClient.phoneNumber)
                                    setFieldValue('email', res.data.personClient.email)
                                    setFieldValue('clientId', res.data.personClient.id)
                                    toastInfo("Wczytano osobę " + res.data.personClient.firstName +
                                        ' ' + res.data.personClient.lastName)
                                }
                            });
                        }
                    }, 500)
                }, [setFieldValue, values.isCompany, values.nip, values.pesel, values.regon])

                useEffect(() => {
                    if (!values.clientId && isValid){
                        const debounced = debouncedFindClient();
                        debounced();
                        return () => debounced.cancel()
                    }
                }, [debouncedFindClient, isValid, values.clientId, values.nip, values.pesel, values.regon]);

                return <Form>
                    <ValidationMessage fieldName="clientId" />
                    <CheckboxInputGroup name="isCompany" label="Czy klient jest firmą?"/>
                    {!values.isCompany && <>
                        <TypeInputGroup name="firstName" label="Imię" type="text" />
                        <TypeInputGroup name="lastName" label="Nazwisko" type="text" />
                        <TypeInputGroup name="pesel" label="PESEL" type="text" />
                    </>}
                    {!!values.isCompany && <>
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