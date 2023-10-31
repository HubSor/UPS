import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { SubProductDto } from "../../api/Dtos";
import { Button, Modal } from "react-bootstrap";
import { TypeInputGroup, SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";
import { AssignSubProductRequest, EditSubProductAssignmentRequest } from "../../api/ApiRequests";
import { number, object } from "yup";

type AssignSubProductModalProps = {
    onSuccess: () => void
    close: () => void
    assignedSubProduct: SubProductDto
    productId: number
}

const assignSubProductSchema = object<EditSubProductAssignmentRequest>().shape({
    price: number()
        .max(1_000_000_000, "Zbyt wysoka cena")
        .required("Pole wymagane")
        .min(0, "Zbyt niska cena"),
})

export function AssignSubProductModal({ onSuccess, close, assignedSubProduct, productId }: AssignSubProductModalProps) {
    const initialValues: AssignSubProductRequest = {
        price: assignedSubProduct.basePrice,
        subProductId: assignedSubProduct.id,
        productId: productId
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            validationSchema={assignSubProductSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Przypisano podprodukt') 
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                Api.AssignSubProduct(v).then(res => handleApiResponse(res))
            }}
        >
            {({ isSubmitting }) => <Form>
                <Modal.Header className="darkblue">
                    <Modal.Title>
                        {"Przypisz podprodukt"}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <TypeInputGroup name="price" label="Cena" type="number"/>
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