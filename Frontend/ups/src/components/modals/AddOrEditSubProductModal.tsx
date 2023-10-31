import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { SubProductDto } from "../../api/Dtos";
import { Button, Modal } from "react-bootstrap";
import { TypeInputGroup, AsInputGroup, SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";
import { EditSubProductRequest } from "../../api/ApiRequests";
import { number, object, string } from "yup";
import { useEffect } from "react";

type AddOrEditSubProductModalProps = {
    onSuccess: () => void
    close: () => void
    editedSubProduct?: SubProductDto
}

const addOrEditSubProductSchema = object<EditSubProductRequest>().shape({
    code: string()
        .min(1, "Zbyt krótki kod")
        .max(6, "Zbyt długi kod")
        .required("Należy podać kod"),
    name: string()
        .required("Pole wymagane")
        .max(128, "Zbyt długa nazwa"),
    basePrice: number()
        .max(1_000_000_000, "Zbyt wysoka cena")
        .required("Pole wymagane")
        .min(0, "Zbyt niska cena"),
    description: string()
        .nullable()
        .max(1000, "Opis zbyt długi")
})

export function AddOrEditSubProductModal({ onSuccess, close, editedSubProduct }: AddOrEditSubProductModalProps) {
    const editMode = !!editedSubProduct;

    const initialValues: EditSubProductRequest = {
        code: editedSubProduct?.code ?? "",
        name: editedSubProduct?.name ?? "",
        basePrice: editedSubProduct?.basePrice ?? 0,
        description: editedSubProduct?.description,
        id: editedSubProduct?.id ?? -1
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            validationSchema={addOrEditSubProductSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>, edit: boolean) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        edit ? toastInfo('Edytowano podprodukt ' + editedSubProduct?.name) : toastInfo('Dodano podprodukt')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }
                
                editMode ?
                    Api.EditSubProduct(v).then(res => handleApiResponse(res, true)) :
                    Api.AddSubProduct(v).then(res => handleApiResponse(res, false))
            }}
        >
            {function FormInner({ isSubmitting, values, setFieldValue }) { 
                useEffect(() => {
                    if (values.code.toUpperCase() !== values.code)
                        setFieldValue("code", values.code.toUpperCase())
                }, [values.code, setFieldValue]);
                
                return <Form>
                    <Modal.Header className="darkblue">
                        <Modal.Title>
                            {editMode ? "Edytuj podprodukt" : "Dodaj podprodukt"}
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <TypeInputGroup name="name" label="Nazwa" type="text"/>
                        <ValidationMessage fieldName="id" />
                        <TypeInputGroup name="code" label="Kod" type="text"/>
                        <TypeInputGroup name="basePrice" label="Podstawowa cena" type="number"/>
                        <AsInputGroup name="description" label="Opis" as="textarea"/>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button type="submit" disabled={isSubmitting}>
                            Zapisz
                        </Button>
                        &nbsp;
                        <Button type="button" variant="danger" onClick={close}>
                            Anuluj
                        </Button>
                    </Modal.Footer>
                </Form>}
            }
        </Formik>
    </Modal>
}