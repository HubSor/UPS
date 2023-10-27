import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { Button, Modal } from "react-bootstrap";
import { SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { DeleteProductRequest } from "../../api/ApiRequests";
import { ProductDto } from "../../api/Dtos";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";

type DeleteProductModalProps = {
    onSuccess: () => void
    close: () => void
    deletedProduct: ProductDto
}

export function DeleteProductModal({ onSuccess, close, deletedProduct }: DeleteProductModalProps) {
    const initialValues: DeleteProductRequest = {
        productId: deletedProduct.id
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Usunięto produkt')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                Api.DeleteProduct(v).then(handleApiResponse)
            }}
        >
            {({ isSubmitting }) => <Form>
                <Modal.Header className="darkred">
                    <Modal.Title>
                        Usuń produkt {deletedProduct.name} {deletedProduct.code}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Czy na pewno chcesz usunąć produkt {deletedProduct.name} wraz z jego parametrami? Podprodukty przypisane do tego produktu nie zostaną usunięte. 
                    <br/>
                    <ValidationMessage fieldName="productId" />
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