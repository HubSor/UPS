import { Field, FieldArray, Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { RoleEnum, UserDto } from "../../api/Dtos";
import { ChangeEvent } from "react";
import { Button, Modal, Form as BForm } from "react-bootstrap";
import { TypeInputGroup, SeparateErrors, ValidationMessage } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";
import { EditUserRequest } from "../../api/ApiRequests";
import { object, string } from "yup";
import { GetRoleDisplayName } from "../../helpers/EnumHelpers";

type AddOrEditUserModalProps = {
    onSuccess: () => void
    close: () => void
    editedUser?: UserDto
}

const addOrEditUserSchema = object<EditUserRequest>().shape({
    username: string()
        .max(64, "Zbyt długi login")
        .min(4, "Zbyt krótki login")
        .required("Należy podać login"),
    password: string()
        .max(128, "Zbyt długie hasło")
})

const roles = [
    RoleEnum.Administrator,
    RoleEnum.UserManager,
    RoleEnum.Seller,
    RoleEnum.ProductManager,
    RoleEnum.ClientManager,
    RoleEnum.SaleManager,
];

const rolesToAddFrom = roles.map(r => ({
    roleId: r,
    name: GetRoleDisplayName(r)
}))

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
            validationSchema={addOrEditUserSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>, edit: boolean) => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                        edit ? toastInfo('Edytowano użytkownika ' + editedUser?.username) : toastInfo('Dodano użytkownika')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                editMode ?
                    Api.EditUser({ ...v, id: editedUser.id }).then(res => handleApiResponse(res, true)) :
                    Api.AddUser(v).then(res => handleApiResponse(res, false))
                
                fh.setSubmitting(false);
            }}
        >
            {({values, isSubmitting}) => <Form>
                <Modal.Header className="darkblue">
                    <Modal.Title>
                        {editMode ? "Edytuj użytkownika" : "Dodaj użytkownika"}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <TypeInputGroup name="username" label="Login" type="text"/>
                    <ValidationMessage fieldName="id" />
                    <TypeInputGroup name="password" label="Hasło" type="password"/>
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