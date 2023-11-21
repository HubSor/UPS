import { Form as FForm, Field, FieldArray, Formik, FormikErrors } from "formik";
import { Api } from "../../api/Api";
import { OptionDto, ParameterDto, ParameterTypeEnum } from "../../api/Dtos";
import { Button, Form, Modal } from "react-bootstrap";
import { TypeInputGroup, SeparateErrors, Option, ValidationMessage, CheckboxInputGroup, AsInputGroup } from "../../helpers/FormHelpers";
import { ApiResponse } from "../../api/ApiResponses";
import { toastDefaultError, toastInfo } from "../../helpers/ToastHelpers";
import { AddParameterRequest, EditParameterRequest } from "../../api/ApiRequests";
import { array, object, string } from "yup";
import { GetParameterTypeDisplayName } from "../../helpers/EnumHelpers";

type AddOrEditParameterModalProps = {
    onSuccess: () => void
    close: () => void
    editedParameter?: ParameterDto
    productId?: number
    subProductId?: number
}

const optionSchema = object<OptionDto>().shape({
    value: string()
        .required("Należy podać wartość")
        .max(256, "Zbyt długa wartość")
})

const addOrEditParameterSchema = object<EditParameterRequest>().shape({
    name: string()
        .required("Pole wymagane")
        .max(128, "Zbyt długa nazwa"),
    options: array().of(optionSchema)
})

const parameterTypeOptions: Option[] = [
    { label: GetParameterTypeDisplayName(ParameterTypeEnum.Text), value: ParameterTypeEnum.Text },
    { label: GetParameterTypeDisplayName(ParameterTypeEnum.Integer), value: ParameterTypeEnum.Integer },
    { label: GetParameterTypeDisplayName(ParameterTypeEnum.Decimal), value: ParameterTypeEnum.Decimal },
    { label: GetParameterTypeDisplayName(ParameterTypeEnum.Checkbox), value: ParameterTypeEnum.Checkbox },
    { label: GetParameterTypeDisplayName(ParameterTypeEnum.TextArea), value: ParameterTypeEnum.TextArea },
    { label: GetParameterTypeDisplayName(ParameterTypeEnum.Select), value: ParameterTypeEnum.Select },
]

export function AddOrEditParameterModal({ onSuccess, close, editedParameter, productId, subProductId }: AddOrEditParameterModalProps) {
    const editMode = !!editedParameter;
    
    const initialValues: EditParameterRequest & AddParameterRequest = {
        parameterId: editedParameter?.id ?? 0,
        name: editedParameter?.name ?? "",
        productId: productId,
        subProductId: subProductId,
        options: editedParameter?.options ?? [],
        required: editedParameter?.required ?? false,
        type: editedParameter?.type ?? ParameterTypeEnum.Text,
    }
    
    return <Modal show size="lg">
        <Formik
            initialValues={initialValues}
            validationSchema={addOrEditParameterSchema}
            onSubmit={(v, fh) => {
                const handleApiResponse = (res: ApiResponse<undefined>, edit: boolean) => {
                    if (res.success && res.data) {
                        onSuccess()
                        close()
                        edit ? toastInfo('Edytowano parametr ' + editedParameter?.name) : toastInfo('Dodano parametr')
                    }
                    else if (res.errors)
                        fh.setErrors(SeparateErrors(res.errors));
                    else
                        toastDefaultError()
                }

                const values = { ...v, type: +v.type };
                editMode ?
                    Api.EditParameter(values).then(res => handleApiResponse(res, true)) :
                    Api.AddParameter(values).then(res => handleApiResponse(res, false))

                fh.setSubmitting(false)
            }}
        >
            {function FormInner({ isSubmitting, values, errors }) {
                return <FForm>
                    <Modal.Header className="darkblue">
                        <Modal.Title>
                            {editMode ? "Edytuj parametr" : "Dodaj parametr"}
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <TypeInputGroup name="name" label="Nazwa" type="text" />
                        <ValidationMessage fieldName="productId" />
                        <ValidationMessage fieldName="subProductId" />
                        <ValidationMessage fieldName="parameterId" />
                        <CheckboxInputGroup name="required" label="Wymagany" />
                        <AsInputGroup name="type" label="Typ" as="select" options={parameterTypeOptions} />
                        <Form.Group className="mb-3">
                            <Form.Label>
                                Opcje
                            </Form.Label>
                            <FieldArray 
                                name="options"
                                render={helpers => <div>
                                    {values.options.map((o, idx) => <div className="m-1 d-flex" key={idx}>
                                        <div className="option-row">
                                            <Field name={`options.${idx}.value`} type="text" className="form-control" />
                                            {!!errors.options?.at(idx) && <div className="error-msg">
                                                {(errors.options?.at(idx) as FormikErrors<OptionDto>).value}
                                            </div>}
                                        </div>
                                        <button className="btn btn-sm btn-outline-danger m-1" type="button"
                                            onClick={() => helpers.remove(idx)}
                                        >
                                            Usuń
                                        </button>
                                    </div>)}
                                    <button className="btn btn-primary m-1" type="button"
                                        onClick={() => helpers.push("")}
                                    >
                                        Dodaj opcję
                                    </button>
                                </div>}
                            />
                        </Form.Group>
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
                </FForm>
            }}
        </Formik>
    </Modal>
}