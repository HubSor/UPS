import { Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { Button, Modal } from "react-bootstrap";
import { SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { DeleteUserRequest } from "../../api/ApiRequests";
import { UserDto } from "../../api/Dtos";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";

type DeleteUserModalProps = {
    onSuccess: () => void
    close: () => void
    deletedUser: UserDto
}

export function DeleteUserModal({ onSuccess, close, deletedUser }: DeleteUserModalProps) {
    const initialValues: DeleteUserRequest = {
        id: deletedUser.id
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        toastInfo('Usunięto użytkownika')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                Api.DeleteUser(v).then(handleApiResponse)
            }}
        >
            {() => <Form>
                <Modal.Header className="darkred">
                    <Modal.Title>
                        Usuń użytkownika {deletedUser.username}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Czy na pewno chcesz usunąć użytkownika {deletedUser.username}?
                    <br/>
                    <ValidationMessage fieldName="id" />
                </Modal.Body>
                <Modal.Footer>
                    <Button type="submit">
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