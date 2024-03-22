// 僅用於Quill的設定

const initialData = {
    // `content` is a Delta object
    // Learn more at: https://quilljs.com/docs/delta
    content: [
        {
            insert:
                '',
        },
    ],
};

const toolbarOptions = [
    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],          // custom dropdown

    ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
    ['blockquote', 'code-block'],
    ['link', 'video'],

    [{ 'header': 1 }, { 'header': 2 }],               // custom button values
    [{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'list': 'check' }],
    [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent

    [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
    [{ 'align': [] }],

    ['clean']                                         // remove formatting button
];


const quill = new Quill('#editor', {
    modules: {
        toolbar: toolbarOptions
    },
    theme: 'snow',
});

const resetForm = () => {
    document.querySelector('[name="postTitle"]').value = initialData.postTitle;
    quill.setContents(initialData.content);
};

resetForm();

const form = document.querySelector('form');
form.addEventListener('formdata', (event) => {
    // Append Quill content before submitting
    event.formData.append('content', JSON.stringify(quill.getContents().ops));
});

document.querySelector('#resetForm').addEventListener('click', () => {
    resetForm();
});