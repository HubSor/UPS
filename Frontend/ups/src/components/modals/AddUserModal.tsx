import { ErrorMessage, Field, FieldArray, Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { AddUserRequest } from "../../api/ApiRequests";
import { RoleEnum } from "../../api/Dtos";
import { ChangeEvent } from "react";
import { Button, Modal, Form as BForm } from "react-bootstrap";
import { InputGroup, SeparateErrors } from "../../helpers/FormHelpers";

const initialValues: AddUserRequest = {
    username: "",
    password: "",
    roleIds: [ 
        RoleEnum.Seller
    ]
}

type AddUserModalProps = {
    onSuccess: () => void
    close: () => void
}

const rolesToAddFrom = [
    { roleId: RoleEnum.Administrator, name: "Administrator" },
    { roleId: RoleEnum.UserManager, name: "Zarządca użytkowników" },
    { roleId: RoleEnum.Seller, name: "Sprzedawca" }
];

export function AddUserModal({ onSuccess, close }: AddUserModalProps) {
    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                Api.AddUser(v).then(res => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                    }
                    else {
                        fh.setErrors(SeparateErrors(res.errors));
                    }
                })
            }}
        >
            {({values}) => <Form>
                <Modal.Header>
                    <Modal.Title>
                        Dodaj użytkownika
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
                        <ErrorMessage name="roleIds" component="div" />
                    </BForm.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button type="submit">
                        Dodaj
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