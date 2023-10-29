import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { ExtendedSubProductDto } from "../../api/Dtos";
import { Button, Modal } from "react-bootstrap";
import { TypeInputGroup, SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";
import { EditSubProductAssignmentRequest } from "../../api/ApiRequests";
import { number, object } from "yup";

type EditAssignedSubProductModalProps = {
    onSuccess: () => void
    close: () => void
    editedSubProduct: ExtendedSubProductDto
    productId: number
}

const editAssignedSubProductSchema = object<EditSubProductAssignmentRequest>().shape({
    newPrice: number()
        .max(1_000_000_000, "Zbyt wysoka cena")
        .required("Pole wymagane")
        .min(0, "Zbyt niska cena"),
})

export function EditAssignedSubProductModal({ onSuccess, close, editedSubProduct, productId }: EditAssignedSubProductModalProps) {
    const initialValues: EditSubProductAssignmentRequest = {
        newPrice: editedSubProduct.price,
        subProductId: editedSubProduct.id,
        productId: productId
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            validationSchema={editAssignedSubProductSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Edytowano przypisanie podproduktu') 
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                Api.EditSubProductAssignment(v).then(res => handleApiResponse(res))
            }}
        >
            {({ isSubmitting }) => <Form>
                <Modal.Header className="darkblue">
                    <Modal.Title>
                        {"Edytuj przypisamy podprodukt"}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <TypeInputGroup name="newPrice" label="Nowa cena" type="number"/>
                    <ValidationMessage fieldName="productId" />
                    <ValidationMessage fieldName="subProductId" />
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
        </Formik>
    </Modal>
}