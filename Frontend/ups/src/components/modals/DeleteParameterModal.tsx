import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { Button, Modal } from "react-bootstrap";
import { SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { DeleteParameterRequest } from "../../api/ApiRequests";
import { ParameterDto } from "../../api/Dtos";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";

type DeleteParameterModalProps = {
    onSuccess: () => void
    close: () => void
    deletedParameter: ParameterDto
}

export function DeleteParameterModal({ onSuccess, close, deletedParameter }: DeleteParameterModalProps) {
    const initialValues: DeleteParameterRequest = {
        parameterId: deletedParameter.id
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Usunięto parametr')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                Api.DeleteParameter(v).then(handleApiResponse)

                fh.setSubmitting(false);
            }}
        >
            {({ isSubmitting }) => <Form>
                <Modal.Header className="darkred">
                    <Modal.Title>
                        Usuń paramter {deletedParameter.name}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Czy na pewno chcesz usunąć parametr {deletedParameter.name}? 
                    <br/>
                    <ValidationMessage fieldName="parameterId" />
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