import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { ProductDto, ProductStatusEnum } from "../../api/Dtos";
import { Button, Modal } from "react-bootstrap";
import { TypeInputGroup, Option, GetProductStatusDisplayName, AsInputGroup, SeparateErrors, ValidationMessage, CheckboxInputGroup } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";
import { EditProductRequest } from "../../api/ApiRequests";
import { boolean, number, object, string } from "yup";
import { useEffect } from "react";

type AddOrEditProductModalProps = {
    onSuccess: () => void
    close: () => void
    editedProduct?: ProductDto
}

const addOrEditProductSchema = object<EditProductRequest>().shape({
    anonymousSaleAllowed: boolean()
        .required("Pole wymagane"),
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

const productOptions: Option[] = [
    { label: GetProductStatusDisplayName(ProductStatusEnum.NotOffered), value: ProductStatusEnum.NotOffered },
    { label: GetProductStatusDisplayName(ProductStatusEnum.Offered), value: ProductStatusEnum.Offered },
    { label: GetProductStatusDisplayName(ProductStatusEnum.Withdrawn), value: ProductStatusEnum.Withdrawn },
]

export function AddOrEditProductModal({ onSuccess, close, editedProduct }: AddOrEditProductModalProps) {
    const editMode = !!editedProduct;

    const initialValues: EditProductRequest = {
        anonymousSaleAllowed: editedProduct?.anonymousSaleAllowed ?? false,
        code: editedProduct?.code ?? "",
        name: editedProduct?.name ?? "",
        basePrice: editedProduct?.basePrice ?? 0,
        description: editedProduct?.description,
        status: editedProduct?.status ?? ProductStatusEnum.Offered,
        id: editedProduct?.id ?? -1
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            validationSchema={addOrEditProductSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>, edit: boolean) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        edit ? toastInfo('Edytowano produkt ' + editedProduct?.name) : toastInfo('Dodano produkt')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }
                
                const values = { ...v, status: +v.status};
                editMode ?
                    Api.EditProduct(values).then(res => handleApiResponse(res, true)) :
                    Api.AddProduct(values).then(res => handleApiResponse(res, false))
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
                            {editMode ? "Edytuj produkt" : "Dodaj produkt"}
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <TypeInputGroup name="name" label="Nazwa" type="text"/>
                        <ValidationMessage fieldName="id" />
                        <TypeInputGroup name="code" label="Kod" type="text"/>
                        <TypeInputGroup name="basePrice" label="Podstawowa cena" type="number"/>
                        {editMode && <AsInputGroup name="status" label="Status" as="select" options={productOptions}/>}
                        <CheckboxInputGroup name="anonymousSaleAllowed" label="Anonimowa sprzedaż"/>
                        <AsInputGroup rows={3} name="description" label="Opis" as="textarea"/>
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