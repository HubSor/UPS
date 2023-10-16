import { Field, FieldArray, Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { RoleEnum, UserDto } from "../../api/Dtos";
import { ChangeEvent } from "react";
import { Button, Modal, Form as BForm } from "react-bootstrap";
import { InputGroup, SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";

type AddOrEditUserModalProps = {
    onSuccess: () => void
    close: () => void
    editedUser?: UserDto
}

const rolesToAddFrom = [
    { roleId: RoleEnum.Administrator, name: "Administrator" },
    { roleId: RoleEnum.UserManager, name: "Zarządca użytkowników" },
    { roleId: RoleEnum.Seller, name: "Sprzedawca" }
];

export function AddOrEditUserModal({ onSuccess, close, editedUser }: AddOrEditUserModalProps) {
    const editMode = !!editedUser;

    const editedUserRoles = editedUser?.roles.map(r => RoleEnum[r as keyof typeof RoleEnum])

    const initialValues = {
        username: editedUser?.username ?? "",
        password: "",
        roleIds: editedUserRoles ?? [ 
            RoleEnum.Seller
        ],
        id: editedUser?.id ?? -1
    }

    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>, edit: boolean) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        edit ? toastInfo('Edytowano użytkownika') : toastInfo('Dodano użytkownika')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                editMode ?
                    Api.EditUser({ ...v, id: editedUser.id }).then(res => handleApiResponse(res, true)) :
                    Api.AddUser(v).then(res => handleApiResponse(res, false))
            }}
        >
            {({values}) => <Form>
                <Modal.Header className="darkblue">
                    <Modal.Title>
                        {editMode ? "Edytuj użytkownika" : "Dodaj użytkownika"}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <InputGroup name="username" label="Login" type="text"/>
                    <InputGroup name="password" label="Hasło" type="password"/>
                    <BForm.Group>
                        <FieldArray name="roleIds"
                            render={ah => <>
                                <BForm.Label>
                                    Role
                                </BForm.Label>
                                <div className="mb-3">
                                    {rolesToAddFrom.map((r, idx) => <div key={idx}>
                                        <Field name={`roledIds[${idx}]`} type="checkbox" value={r.roleId}
                                            checked={values.roleIds.includes(r.roleId)}
                                            onChange={(e: ChangeEvent<HTMLInputElement>) => {
                                                if (e.target.checked)
                                                ah.insert(idx, r.roleId)
                                            else
                                            ah.remove(values.roleIds.findIndex(x => x === r.roleId))
                                        }}/>
                                        &nbsp;
                                        <BForm.Label>
                                            {r.name}
                                        </BForm.Label>
                                    </div>)}
                                </div>
                            </>}
                        />
                        <ValidationMessage fieldName="roleIds" />
                    </BForm.Group>
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