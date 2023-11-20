import { Form, Formik, FormikHelpers, FormikProps } from "formik"
import { SalePathFormProps } from "../../pages/SaleMainPage"
import { UpsertClientRequest } from "../../api/ApiRequests"
import { object, string } from "yup"
import { Api } from "../../api/Api"
import { ApiResponse, UpsertClientResponse } from "../../api/ApiResponses"
import { toastError, toastInfo } from "../../helpers/ToastHelpers"
import { CheckboxInputGroup, SeparateErrors, TypeInputGroup, ValidationMessage } from "../../helpers/FormHelpers"
import { useCallback, useEffect, useState } from "react"
import { Button } from "react-bootstrap"
import debounce from 'lodash.debounce';

type FillClientInfoProps = SalePathFormProps

const fillClientInfoSchema = object<UpsertClientRequest>().shape({
    nip: string().nullable()
        .matches(/^[0-9]{10}$/, "Niepoprawny NIP"),
    pesel: string().nullable()
        .matches(/^[0-9]{11}$/, "Niepoprawny PESEL"),
    regon: string().nullable()
        .matches(/^[0-9]{9,14}/, "Niepoprawny REGON")
        .min(9, "Zbyt krótki REGON").max(14, "Zbyt długi REGON"),
    phoneNumber: string().nullable()
        .matches(/^[0-9]{9,15}/, "Niepoprawny numer telefonu")
        .min(9, "Zbyt krótki numer telefonu").max(14, "Zbyt długi numer telefonu"),
    companyName: string().nullable()
        .max(256, "Zbyt długa nazwa firmy"),
    firstName: string().nullable()
        .max(256, "Zbyt długie imię"),
    lastName: string().nullable()
        .max(256, "Zbyt długie nazwisko"),
    email: string().nullable()
        .email("Niepoprawny adres E-mail")
})

const validateMinimalData = (values: UpsertClientRequest, fh: FormikHelpers<UpsertClientRequest>): boolean => {
    if (values.isCompany && !values.companyName){
        fh.setFieldError("companyName", "Należy podać nazwę firmy")
        return false;
    }
    else {
        if (!values.firstName) {
            fh.setFieldError("firstName", "Należy podać imię")
            return false;
        }
        if (!values.lastName) {
            fh.setFieldError("lastName", "Należy podać nazwisko")
            return false;
        }
    }

    return true;
}

export const FillClientInfoForm = ({ state, dispatch }: FillClientInfoProps) => {
    const [editingExistingClient, setEditingExistingClient] = useState(false);

    const initialValues: UpsertClientRequest = {
        isCompany: false,
    }

    return <div>
        <h3>Uzupełnij dane klienta</h3>
        {editingExistingClient && <div className="alert alert-primary">
            Edytujesz klienta wczytanego z systemu
        </div>}
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
                
                if (validateMinimalData(v, fh))
                    Api.UpsertClient(v).then(handleApiResponse)

                fh.setSubmitting(false);
            }}
        >
            {function FormInner({ isSubmitting, values, setFieldValue, isValid }: FormikProps<UpsertClientRequest>) {
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
                                    setEditingExistingClient(true)
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
                                    setEditingExistingClient(true)
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

                useEffect(() => {
                    if (values.isCompany){
                        setFieldValue("firstName", undefined)
                        setFieldValue("pesel", undefined)
                        setFieldValue("lastName", undefined)
                    }
                    else{
                        setFieldValue("companyName", undefined)
                        setFieldValue("regon", undefined)
                        setFieldValue("nip", undefined)
                    }
                    setFieldValue("clientId", undefined)
                    setEditingExistingClient(false)
                }, [setFieldValue, values.isCompany])

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