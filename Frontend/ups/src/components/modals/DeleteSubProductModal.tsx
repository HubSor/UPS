import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { Button, Modal } from "react-bootstrap";
import { SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { DeleteSubProductRequest } from "../../api/ApiRequests";
import { SubProductDto } from "../../api/Dtos";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";

type DeleteSubProductModalProps = {
    onSuccess: () => void
    close: () => void
    deletedSubProduct: SubProductDto
}

export function DeleteSubProductModal({ onSuccess, close, deletedSubProduct }: DeleteSubProductModalProps) {
    const initialValues: DeleteSubProductRequest = {
        subProductId: deletedSubProduct.id
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Usunięto podprodukt')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                Api.DeleteSubProduct(v).then(handleApiResponse)
            }}
        >
            {({ isSubmitting }) => <Form>
                <Modal.Header className="darkred">
                    <Modal.Title>
                        Usuń podprodukt {deletedSubProduct.name} {deletedSubProduct.code}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Czy na pewno chcesz usunąć podprodukt {deletedSubProduct.name} wraz z jego parametrami? Produkty, do których jest przypisany nie zostaną usunięte. 
                    <br/>
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