import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { Button, Modal } from "react-bootstrap";
import { SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { UnassignSubProductsRequest } from "../../api/ApiRequests";
import { ProductDto, SubProductDto } from "../../api/Dtos";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";

type UnassignSubProductModalProps = {
    onSuccess: () => void
    close: () => void
    subProduct: SubProductDto
    product: ProductDto
}

export function UnassignSubProductModal({ onSuccess, close, subProduct, product }: UnassignSubProductModalProps) {
    const initialValues = {
        subProductId: subProduct.id
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Usunięto przypisanie podproduktu')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                const values: UnassignSubProductsRequest = {
                    subProductIds: [ v.subProductId ],
                    productId: product.id
                }

                Api.UnassignSubProducts(values).then(handleApiResponse)
            }}
        >
            {({ isSubmitting }) => <Form>
                <Modal.Header className="darkred">
                    <Modal.Title>
                        Usuń przypisanie {subProduct.code} do {product.code}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Czy na pewno chcesz usunąć przypisanie podproduktu {subProduct.name} do produktu {product.name}? Nie spowoduje to całkowitego usunięcia podproduktu ani produktu.
                    <br/>
                    <ValidationMessage fieldName="subProductIds" />
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