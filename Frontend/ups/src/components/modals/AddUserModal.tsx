import { ErrorMessage, Field, FieldArray, Form, Formik } from "formik";
import { Api } from "../../api/Api";
import { AddUserRequest } from "../../api/ApiRequests";
import { RoleEnum } from "../../api/Dtos";
import React, { ChangeEvent } from "react";

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
    return <div>
        <div>
            Dodaj użytkownika
        </div>
        <Formik
            initialValues={initialValues}
            onSubmit={(v, fh) => {
                Api.AddUser(v).then(res => {
                    if (res.success && res.data){
                        onSuccess()
                        close()
                    }
                    else {
                        fh.setErrors(res.errors);
                    }
                })
            }}
        >
            {({values}) => <Form>
                <div>
                    <label>Login</label>
                    <Field type="text" name="username"/>
                    <ErrorMessage name="username" component="div"/>
                </div>
                <div>
                    <label>Hasło</label>
                    <Field type="password" name="password"/>
                    <ErrorMessage name="password" component="div" />
                </div>
                <div>
                    <FieldArray name="roleIds"
                        render={ah => <>
                            <label>Role</label>
                            <div>
                                {rolesToAddFrom.map((r, idx) => <React.Fragment key={idx}>
                                    <label>{r.name}</label>
                                    <Field name={`roledIds[${idx}]`} type="checkbox" value={r.roleId}
                                        checked={values.roleIds.includes(r.roleId)}
                                        onChange={(e: ChangeEvent<HTMLInputElement>) => {
                                            if (e.target.checked)
                                                ah.insert(idx, r.roleId)
                                            else
                                                ah.remove(values.roleIds.findIndex(x => x === r.roleId))
                                        }}
                                    />
                                </React.Fragment>)}
                            </div>
                        </>}
                    />
                    <ErrorMessage name="password" component="div" />
                </div>
                <nav>
                    <button type="submit">Dodaj</button>
                    <button type="button" onClick={close}>Anuluj</button>
                </nav>
            </Form>}
        </Formik>
    </div>
}